using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Xml.Protected
{
	public sealed class ProviderLoader
	{
		private IProviderCollection _providers;

		public ProviderLoader()
			: this(new ProviderCollection())
		{
		}

		public ProviderLoader(IProviderCollection providers)
		{
			_providers = providers;
		}

		public IProviderCollection Providers
		{
			get { return _providers; }
		}

		public void Append(NameValueCollection parameters)
		{
			string name = parameters["name"];
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name", "missing parameter");
			parameters.Remove("name");
			string type = parameters["type"];
			if(string.IsNullOrWhiteSpace(type))
				throw new ArgumentNullException("type", "missing parameter");
			parameters.Remove("type");

			resolve(name, type, parameters);
		}

		private void resolve(string name, string type, NameValueCollection parameters)
		{
			if (onLoading(name, type, parameters))
				return;

			Type providerType = Type.GetType(type, true);

			var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
			if (provider == null)
				throw new FormatException(string.Format("instance of type `{0}' can not be cast to ProtectedConfigurationProvider", providerType.FullName));

			provider.Initialize(name, parameters);

			_providers.Set(name, provider);
		}

		public ProviderLoader SubscribeLoading(EventHandler<ProviderLoadingEventArgs> handler)
		{
			Loading += handler;
			return this;
		}

		public ProviderLoader NoClearing()
		{
			Clearing += (s, e) => { e.Canceled = true; };
			return this;
		}

		public ProviderLoader SubscribeClearing(EventHandler<CancelableEventArgs> handler)
		{
			Clearing += handler;
			return this;
		}

		public event EventHandler<CancelableEventArgs> Clearing;
		
		private bool onClearing()
		{
			var copy = Clearing;
			if (copy == null)
				return false;

			var args = new CancelableEventArgs() { Canceled = false };
			copy(this, args);
			return args.Canceled;
		}

		public event EventHandler<ProviderLoadingEventArgs> Loading;

		private bool onLoading(string name, string type, NameValueCollection parameters)
		{
			var copy = Loading;
			if (copy == null)
				return false;

			var args = new ProviderLoadingEventArgs() { Canceled = false, Name = name, Type = type, Parameters = parameters };
			copy(this, args);
			return args.Canceled;
		}

		public static IEnumerable<ProviderSettings> ConfigProtectedDataProviders
		{
			get
			{
				var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				if(config == null)
					yield break;

				var section = config.GetSection("configProtectedData") as ProtectedConfigurationSection;
				if(section == null)
					yield break;

				foreach (ProviderSettings settings in section.Providers)
					yield return settings;
			}
		}

		public static ProviderLoader FromAppSettings(IAppSettings settings)
		{
			return new ProviderLoader().LoadAppSettings(settings);
		}

		public ProviderLoader TryLoadAppSettings(IConfigNodeProvider provider)
		{
			var cfg = tryGetConfig(provider);
			if (cfg != null)
				loadConfig(cfg);
			
			return this;
		}

		private ConfigProtectedData tryGetConfig(IConfigNodeProvider nodeProvider)
		{
			foreach (var node in nodeProvider.ByName(typeof(ConfigProtectedData).GetSectionName()))
				return DefaultDeserializer.Instance.Deserialize<ConfigProtectedData>(node);

			return null;
		}

		public ProviderLoader LoadAppSettings(IAppSettings settings)
		{
			loadConfig(settings.TryFirst<ConfigProtectedData>());
			return this;
		}

		private void loadConfig(ConfigProtectedData cfg)
		{
			foreach(var pair in cfg.Providers.Nested)
			{
				if (pair.Key == "clear")
				{
					if (onClearing())
						continue;

					_providers.Clear();
					continue;
				}

				if (pair.Key == "add")
				{
					Append(getNameValueCollection(pair.Value));
					continue;
				}

				throw new InvalidOperationException(string.Format("unexpected element `{0}'", pair.Key));
			}
		}

		private static NameValueCollection getNameValueCollection(ICfgNode node)
		{
			var result = new NameValueCollection();
			foreach (var pair in node.Nested)
				result.Add(pair.Key, pair.Value.Text);
			return result;
		}

		public static ProviderLoader FromConfigProtectedData()
		{
			return new ProviderLoader().LoadConfigProtectedData();
		}

		public ProviderLoader LoadConfigProtectedData()
		{
			foreach (var settings in ConfigProtectedDataProviders)
				resolve(settings.Name, settings.Type, settings.Parameters);

			return this;
		}

		public void TryExtractConfigProtectedData(object s, LoadedEventArgs e)
		{
			TryLoadAppSettings(e.Settings);

			var encr = e.Settings as IXmlEncryptable;
			if(encr != null)
				encr.Providers = Providers;
		}
	}
}
