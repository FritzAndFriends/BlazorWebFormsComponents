using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BlazorWebFormsComponents.LoginControls
{
  public class RoleGroupCollection : IList<RoleGroup>
  {
	private List<RoleGroup> _RoleGroups = new List<RoleGroup>();

	internal RoleGroupCollection() { }

	internal RoleGroup GetRoleGroup(ClaimsPrincipal user)
	{
	  for (int i = 0; i < _RoleGroups.Count; i++)
	  {
		var roleGroup = _RoleGroups[i];
		var roles = roleGroup.Roles.Split(',');
		for (int j = 0; j < roles.Length; j++)
		{
		  if (user.IsInRole(roles[j]))
		  {
				return roleGroup;
		  }
		}
	  }

	  return null;
	}

	#region Collection Features

	public RoleGroup this[int index]
	{
	  get { return _RoleGroups[index]; }
	  set { _RoleGroups[index] = value; }
	}

	public int Count { get { return _RoleGroups.Count; } }

	public bool IsReadOnly { get { return false; } }

	public void Add(RoleGroup item)
	{
	  _RoleGroups.Add(item);
	}

	public void AddAt(int index, RoleGroup item)
	{
	  Insert(index, item);
	}

	public void Clear()
	{
	  _RoleGroups.Clear();
	}

	public bool Contains(RoleGroup item)
	{
	  return _RoleGroups.Contains(item);
	}

	public void CopyTo(RoleGroup[] array, int arrayIndex)
	{
	  _RoleGroups.CopyTo(array, arrayIndex);
	}

	public IEnumerator<RoleGroup> GetEnumerator()
	{
	  return _RoleGroups.GetEnumerator();
	}

	public int IndexOf(RoleGroup item)
	{
	  return _RoleGroups.IndexOf(item);
	}

	public void Insert(int index, RoleGroup item)
	{
	  _RoleGroups.Insert(index, item);
	}

	public bool Remove(RoleGroup item)
	{
	  return _RoleGroups.Remove(item);
	}

	public void RemoveAt(int index)
	{
	  _RoleGroups.RemoveAt(index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
	  return _RoleGroups.GetEnumerator();
	}

	#endregion

  }
}
