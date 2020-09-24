using System.Collections.Generic;
using System.Drawing;
using Mtgdb.Controls;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SelectionStateTests
	{
		[Test]
		public void When_direction_is_negative_Then_top_left_is_shifted()
		{
			var state = new RectangularSelection();

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -10));

			var selection = state.Rectangle;

			Assert.That(selection.Top, Is.EqualTo(-10));
			Assert.That(selection.Left, Is.EqualTo(-10));
			Assert.That(selection.Right, Is.EqualTo(0));
			Assert.That(selection.Bottom, Is.EqualTo(0));
		}

		[Test]
		public void When_new_state_does_not_intersect_previous_Then_delta_is_previous_and_current_rectangle()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 10));
			state.MoveTo(new Point(-10, -10));

			var expectedDelta = Sequence.Array(new Rectangle(0, 0, 10, 10), new Rectangle(-10, -10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_is_greater_than_old_Then_delta_is_right_and_bottom_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 10));
			state.MoveTo(new Point(20, 20));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 20), new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_is_less_than_old_Then_delta_is_right_and_bottom_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(20, 20));
			state.MoveTo(new Point(10, 10));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 20), new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_increases_width_and_decreases_height_Then_delta_is_right_and_bottom_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 20));
			state.MoveTo(new Point(20, 10));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 20), new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_decreases_width_and_increases_height_Then_delta_is_right_and_bottom_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(20, 10));
			state.MoveTo(new Point(10, 20));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 20), new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_increases_width_Then_delta_is_right_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 10));
			state.MoveTo(new Point(20, 10));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_increases_height_Then_delta_is_bottom_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 10));
			state.MoveTo(new Point(10, 20));

			var expectedDelta = Sequence.Array(new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_decreases_width_Then_delta_is_right_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(20, 10));
			state.MoveTo(new Point(10, 10));

			var expectedDelta = Sequence.Array(new Rectangle(10, 0, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_positive_state_decreases_height_Then_delta_is_bottom_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(10, 20));
			state.MoveTo(new Point(10, 10));

			var expectedDelta = Sequence.Array(new Rectangle(0, 10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_is_greater_than_old_Then_delta_is_left_and_top_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -10));
			state.MoveTo(new Point(-20, -20));

			var expectedDelta = Sequence.Array(new Rectangle(-20, -20, 10, 20), new Rectangle(-10, -20, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_is_less_than_old_Then_delta_is_left_and_top_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-20, -20));
			state.MoveTo(new Point(-10, -10));

			var expectedDelta = Sequence.Array(new Rectangle(-20, -20, 10, 20), new Rectangle(-10, -20, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_increases_width_and_decreases_height_Then_delta_is_left_and_top_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -20));
			state.MoveTo(new Point(-20, -10));

			var expectedDelta = Sequence.Array(new Rectangle(-20, -20, 10, 20), new Rectangle(-10, -20, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_decreases_width_and_increases_height_Then_delta_is_left_and_top_increments()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-20, -10));
			state.MoveTo(new Point(-10, -20));

			var expectedDelta = Sequence.Array(new Rectangle(-20, -20, 10, 20), new Rectangle(-10, -20, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_increases_width_Then_delta_is_left_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -10));
			state.MoveTo(new Point(-20, -10));

			var expectedDelta =  Sequence.Array(new Rectangle(-20, -10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_increases_height_Then_delta_is_top_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -10));
			state.MoveTo(new Point(-10, -20));

			var expectedDelta = new[]
			{
				new Rectangle(-10, -20, 10, 10)
			};

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_decreases_width_Then_delta_is_left_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (r, l, s, d) => delta = d;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-20, -10));
			state.MoveTo(new Point(-10, -10));

			var expectedDelta =  Sequence.Array(new Rectangle(-20, -10, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}

		[Test]
		public void When_new_negative_state_decreases_height_Then_delta_is_top_increment()
		{
			IEnumerable<Rectangle> delta = null;
			var state = new RectangularSelection();

			state.Changed += (previousRect, previousStart, previousSelecting, invalidateAreas) => delta = invalidateAreas;

			state.StartAt(new Point(0, 0));
			state.MoveTo(new Point(-10, -20));
			state.MoveTo(new Point(-10, -10));

			var expectedDelta =  Sequence.Array(new Rectangle(-10, -20, 10, 10));

			Assert.That(delta, Is.EquivalentTo(expectedDelta));
		}
	}
}
