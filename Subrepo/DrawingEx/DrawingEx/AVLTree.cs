using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
/*
 * AVL Tree class
 * 22-11-2008
 * Varisoft Industries
 */
namespace DrawingEx
{
	/// <summary>
	/// interface defining operations every tree
	/// has to implement
	/// </summary>
	public interface ITree<K, T> where K:IComparable
	{
		/// <summary>
		/// insert a value and return success
		/// </summary>
		bool Insert(K key, T value);
		/// <summary>
		/// remove value and return success
		/// </summary>
		bool Remove(K key);
		/// <summary>
		/// search for value and return success
		/// </summary>
		bool Search(K key);
	}
	/// <summary>
	/// implements an AVL tree
	/// </summary>
	/// <typeparam name="K">Type of Keys</typeparam>
	/// <typeparam name="T">Type of Values (can be the same as K)</typeparam>
	public class AVLTree<K, T>:ITree<K, T> where K:IComparable
	{
		#region types
		/// <summary>
		/// specifies the order of the tree nodes
		/// on flattened output
		/// </summary>
		public enum TreeOrder
		{
			PreOrder,
			PostOrder,
			InOrder
		}
		/// <summary>
		/// drawing node for painting a tree onto a
		/// graphics object
		/// </summary>
		public class DrawingNode<TKey,TValue> where TKey:IComparable
		{
			private static readonly Size BulletSize = new Size(20, 20);
			#region variables
			private TKey _key;
			private DrawingNode<TKey,TValue> _left, _right;
			private int _height;
			#endregion
			#region ctor
			//ctor
			private DrawingNode(TKey value, DrawingNode<TKey, TValue> left,
				DrawingNode<TKey, TValue> right, int height)
			{
				_key = value;
				_left = left;
				_right = right;
				_height = height;
			}
			/// <summary>
			/// creates a drawing node which represents the
			/// root node with all subnodes of the avl tree
			/// </summary>
			public static DrawingNode<TKey,TValue> FromAVLTree(AVLTree<TKey,TValue> tree)
			{
				if (tree == null) return null;
				return FromNode(tree._root);
			}
			//helper method
			private static DrawingNode<TKey,TValue> FromNode(AVLTree<TKey,TValue>.Node<TKey,TValue> node)
			{
				if (node == null) return null;
				return new DrawingNode<TKey,TValue>(node.Key,
					FromNode(node.Left), FromNode(node.Right), node.Height);
			}
			#endregion
			/// <summary>
			/// gets the preffered size of the tree
			/// </summary>
			public Size GetRequiredSize()
			{
				return new Size(
					BulletSize.Width * (int)Math.Pow(2, _height),
					BulletSize.Height * 2 * _height);
			}
			public void Draw(Graphics gr, RectangleF bounds)
			{
				if (gr == null) return;
				SizeF space = new SizeF(bounds.Width / 2f,
						bounds.Height / (float)(_height+1));
				using (StringFormat fmt = new StringFormat())
				{
					fmt.Alignment = fmt.LineAlignment = StringAlignment.Center;
					DrawInternal(gr, fmt, new RectangleF(
						bounds.X + bounds.Width / 2f - BulletSize.Width / 2f,
						bounds.Y + bounds.Height / 2f - (space.Height * (float)(_height) + BulletSize.Height) / 2f,
						BulletSize.Width, BulletSize.Height), space);
				}
			}
			private void DrawInternal(Graphics gr, StringFormat fmt, RectangleF bounds, SizeF space)
			{
				float w2 = bounds.Width / 2f,
					h2 = bounds.Height / 2f,
					s2 = space.Width / 2f;
				if (_left != null)
				{
					gr.DrawLine(Pens.Black,
						bounds.X + w2, bounds.Y + h2,
						bounds.X - s2 + w2, bounds.Y + h2 + space.Height);

					_left.DrawInternal(gr,fmt,
						new RectangleF(bounds.X - s2,
						bounds.Y + space.Height, bounds.Width, bounds.Height),
						new SizeF(s2, space.Height));
				}
				if (_right != null)
				{
					gr.DrawLine(Pens.Black,
						bounds.X + w2, bounds.Y + h2,
						bounds.X + s2 + w2, bounds.Y + h2 + space.Height);

					_right.DrawInternal(gr,fmt,
						new RectangleF(bounds.X + s2,
						bounds.Y + space.Height, bounds.Width, bounds.Height),
						new SizeF(s2, space.Height));
				}
				//draw content
				gr.FillEllipse(Brushes.White, bounds);
				gr.DrawEllipse(Pens.Black, bounds);

				gr.DrawString(_key.ToString(), System.Windows.Forms.Control.DefaultFont,
					Brushes.Black, bounds, fmt);
				//                gr.DrawString(_height.ToString(), System.Windows.Forms.Control.DefaultFont,
				//Brushes.Blue, bounds, fmt);
			}
		}
		/// <summary>
		/// represents a data node
		/// </summary>
		private class Node<TKey, TValue>
		{
			#region variables
			private TKey _key;
			public int Height = 0;
			public Node<TKey, TValue> Left, Right;
			public TValue Value;
			#endregion
			public Node(TKey key, TValue value)
			{
				_key=key;
				Value = value;
				Left = null;
				Right = null;
			}
			#region properties
			public TKey Key
			{
				get { return _key; }
			}
			#endregion
			public override string ToString()
			{
				return _key.ToString();
			}
		}
		#endregion
		#region variables
		private Node<K, T> _root;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public AVLTree()
		{
			_root = null;
		}
		#region helpers
		//return height of element or -1 if NULL
		private int height(Node<K, T> node)
		{
			return node == null ? -1 : node.Height;
		}
		//performs a double rotation with left child
		private void rotateDoubleLeft(ref Node<K, T> node)
		{
			rotateRight(ref node.Right);
			rotateLeft(ref node);
		}
		//performs a simple rotation with left child
		private void rotateLeft(ref Node<K, T> node)
		{
			Node<K, T> root = node.Right;
			node.Right = node.Right.Left;
			root.Left = node;
			//recalc heights
			root.Left.Height = Math.Max(height(root.Left.Left),
				height(root.Left.Right)) + 1;
			root.Height = Math.Max(height(root.Right),
				height(root.Left));
			//copy
			node = root;
		}
		//performs a double rotation with right child
		private void rotateDoubleRight(ref Node<K, T> node)
		{
			rotateLeft(ref node.Left);
			rotateRight(ref node);
		}
		//performs a simple rotation with right child
		private void rotateRight(ref Node<K, T> node)
		{
			Node<K, T> root = node.Left;
			node.Left = node.Left.Right;
			root.Right = node;
			//recalc heights
			root.Right.Height = Math.Max(height(root.Right.Left),
				height(root.Right.Right)) + 1;
			root.Height = Math.Max(height(root.Right),
				height(root.Left));
			//copy
			node = root;
		}
		//inserts a key/value pair and returns success
		private bool insert(K key, T value, ref Node<K, T> root)
		{
			//empty element, add value
			if (root == null)
			{
				root = new Node<K, T>(key, value);
				return true;
			}
			bool flag = false;
			//search for place
			switch (key.CompareTo(root.Key))
			{
				case 1:
					#region key > root.key => Right
					flag = insert(key, value, ref root.Right);
					if (height(root.Right) > (height(root.Left) + 1))
					{
						if (height(root.Right.Left) > height(root.Right.Right))
							rotateDoubleLeft(ref root);//inner tree is heigher
						else
							rotateLeft(ref root);
					}
					break;
					#endregion
				case -1:
					#region key < root.key => Left
					flag = insert(key, value, ref root.Left);
					if (height(root.Left) > (height(root.Right) + 1))
					{
						if (height(root.Left.Left) < height(root.Left.Right))
							rotateDoubleRight(ref root);//inner tree is heigher
						else
							rotateRight(ref root);
					}
					break;
					#endregion
				default: return false;//match found
			}
			//recalc heights
			root.Height = Math.Max(height(root.Left),
				height(root.Right)) + 1;
			return flag;
		}
		//removes a key and returns success
		private bool remove(K key, ref Node<K, T> root)
		{
			//element not found
			if (root == null) return false;
			bool flag = true;

			switch (key.CompareTo(root.Key))
			{
				case 1:
					#region key > root.key => Right
					flag = remove(key, ref root.Right);
					if (height(root.Right) + 1 < height(root.Left))
					{
						if (height(root.Left.Left) < height(root.Left.Right))
							rotateDoubleRight(ref root);//inner tree is heigher
						else
							rotateRight(ref root);
					}
					break;
					#endregion
				case -1:
					#region key < root.key => Left
					flag = remove(key, ref root.Left);
					if (height(root.Left) + 1 < height(root.Right))
					{
						if (height(root.Right.Left) > height(root.Right.Right))
							rotateDoubleLeft(ref root);//inner tree is heigher
						else
							rotateLeft(ref root);
					}
					break;
					#endregion
				default:
					#region key = root.key => match found
					Node<K,T> replace;
					//search for subtrees to be rearranged
					if (root.Right == null)
						replace = root.Left;
					else if (root.Left == null)
						replace = root.Right;
					else
					{
						//search symmetrical next
						Node<K,T> symnext = root.Right;
						while (symnext.Left != null)
							symnext = symnext.Left;
						replace = symnext;
						//remove and add old subtrees
						remove(symnext.Key, ref root);
						replace.Left = root.Left;
						replace.Right = root.Right;
					}
					root = replace;
					break;
					#endregion
			}
			//recalc heights
			if (root != null)
				root.Height = Math.Max(height(root.Left),
					height(root.Right)) + 1;
			return flag;
		}
		//searches a key
		private Node<K, T> search(K key)
		{
			Node<K, T> node = _root;
			while (node != null)
			{
				switch (key.CompareTo(node.Key))
				{
					case 0: return node;
					case 1: node = node.Right; break;
					case -1: node = node.Left; break;
				}
			}
			return null;
		}
		//appends nodes to list
		private void AppendNodes(Node<K, T> node, List<T> values, TreeOrder order)
		{
			if (node == null) return;
			switch (order)
			{
				case TreeOrder.InOrder:
					AppendNodes(node.Left, values, order);
					values.Add(node.Value);
					AppendNodes(node.Right, values, order);
					break;
				case TreeOrder.PreOrder:
					values.Add(node.Value);
					AppendNodes(node.Left, values, order);
					AppendNodes(node.Right, values, order);
					break;
				case TreeOrder.PostOrder:
					AppendNodes(node.Left, values, order);
					AppendNodes(node.Right, values, order);
					values.Add(node.Value);
					break;
			}
		}
		//appends nodes to list iteratively
		private void AppendNodesIter(Node<K, T> node, List<T> values)
		{
			Stack<Node<K,T>> stack = new Stack<Node<K,T>>();
			//inorder:
			do
			{
				while (node != null)
				{
					stack.Push(node);
					node = node.Left;
				}
				node = stack.Pop();
				values.Add(node.Value);
				node = node.Right;
			} while (stack.Count > 0 || node != null);
		}
		#endregion
		#region members
		/// <summary>
		/// searches for value and returns TRUE if found in tree
		/// </summary>
		public bool Search(K key)
		{
			return search(key)!=null;
		}
		/// <summary>
		/// insert value and return FALSE if already in tree
		/// </summary>
		public bool Insert(K key, T value)
		{
			return insert(key, value, ref _root);
		}
		/// <summary>
		/// removes a value and returns FALSE if value is not in tree
		/// </summary>
		/// <param name="value"></param>
		public bool Remove(K key)
		{
			return remove(key, ref _root);
		}
		/// <summary>
		/// returns an array representation of the tree
		/// </summary>
		public T[] ToArray(TreeOrder order)
		{
			if (_root == null) return null;
			List<T> values = new List<T>();
			AppendNodes(_root, values, order);
			return values.ToArray();
		}
		#endregion
	}
}
