using System;
using System.Collections.Generic;
using BlazorWebFormsComponents.Interfaces;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A collection of <see cref="GridViewRow{T}"/> objects that extends
	/// <c>List&lt;IRow&lt;T&gt;&gt;</c>. The typed indexer returns <see cref="GridViewRow{T}"/>
	/// directly, matching the Web Forms <c>GridViewRowCollection</c> pattern where
	/// <c>CartList.Rows[i].FindControl("X")</c> works without a cast.
	/// </summary>
	public class GridViewRowCollection<ItemType> : List<IRow<ItemType>>
	{
		/// <summary>
		/// Gets the <see cref="GridViewRow{T}"/> at the specified index.
		/// </summary>
		public new GridViewRow<ItemType> this[int index]
		{
			get
			{
				var row = base[index];
				if (row is GridViewRow<ItemType> gvr)
					return gvr;

				throw new InvalidCastException(
					$"Row at index {index} is {row?.GetType().Name}, not GridViewRow<{typeof(ItemType).Name}>.");
			}
			set => base[index] = value;
		}
	}
}

