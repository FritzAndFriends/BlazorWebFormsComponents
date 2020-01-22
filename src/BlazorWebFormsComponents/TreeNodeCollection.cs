using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

	public class TreeNodeCollection : IList<TreeNode>
	{

		private List<TreeNode> _Nodes = new List<TreeNode>();

		internal TreeNodeCollection() { }

		#region Collection Features

		public TreeNode this[int index] {
			get { return _Nodes[index]; }
			set { _Nodes[index] = value; }
		}

		public int Count { get { return _Nodes.Count; } }

		public bool IsReadOnly { get { return false; } }

		public void Add(TreeNode item)
		{
			_Nodes.Add(item);
		}

		public void AddAt(int index, TreeNode item) {
			Insert(index, item);
		}

		public void Clear()
		{
			_Nodes.Clear();
		}

		public bool Contains(TreeNode item)
		{
			return _Nodes.Contains(item);
		}

		public void CopyTo(TreeNode[] array, int arrayIndex)
		{
			_Nodes.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TreeNode> GetEnumerator()
		{
			return _Nodes.GetEnumerator();
		}

		public int IndexOf(TreeNode item)
		{
			return _Nodes.IndexOf(item);
		}

		public void Insert(int index, TreeNode item)
		{
			_Nodes.Insert(index, item);
		}

		public bool Remove(TreeNode item)
		{
			return _Nodes.Remove(item);
		}

		public void RemoveAt(int index)
		{
			_Nodes.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Nodes.GetEnumerator();
		}

		#endregion

	}

	public class TreeNodeEventArgs : EventArgs {

		public TreeNodeEventArgs(TreeNode node)
		{
			this.Node = node;
		}

		public TreeNode Node { get; private set; }

	}

}
