﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{

	public abstract class TreeViewImageSet
	{

		public static ArrowsTreeViewImageSet Arrows { get; } = new ArrowsTreeViewImageSet();
		public static BulletedListTreeViewImageSet BulletedList { get; } = new BulletedListTreeViewImageSet() { HasImages_Collapse = false };
		public static BulletedList2TreeViewImageSet BulletedList2 { get; } = new BulletedList2TreeViewImageSet() { HasImages_Collapse = false };
		public static BulletedList3TreeViewImageSet BulletedList3 { get; } = new BulletedList3TreeViewImageSet() { HasImages_Collapse = false };
		public static BulletedList4TreeViewImageSet BulletedList4 { get; } = new BulletedList4TreeViewImageSet() { HasImages_Collapse = false };
		public static ContactsTreeViewImageSet Contacts { get; } = new ContactsTreeViewImageSet();
		public static DefaultTreeViewImageSet Default { get; } = new DefaultTreeViewImageSet();
		public static EventsTreeViewImageSet Events { get; } = new EventsTreeViewImageSet() { HasImages_Collapse = false };
		public static FAQTreeViewImageSet FAQ { get; } = new FAQTreeViewImageSet() { HasImages_Collapse = false };
		public static InboxTreeViewImageSet Inbox { get; } = new InboxTreeViewImageSet() { HasImages_Collapse = false };
		public static MSDNTreeViewImageSet MSDN { get; } = new MSDNTreeViewImageSet();
		public static NewsTreeViewImageSet News { get; } = new NewsTreeViewImageSet() { HasImages_Collapse = false };
		public static SimpleTreeViewImageSet Simple { get; } = new SimpleTreeViewImageSet() { HasImages_Collapse = false };
		public static Simple2TreeViewImageSet Simple2 { get; } = new Simple2TreeViewImageSet() { HasImages_Collapse = false };
		public static WindowsHelpTreeViewImageSet WindowsHelp { get; } = new WindowsHelpTreeViewImageSet();
		public static XP_ExplorerTreeViewImageSet XP_Explorer { get; } = new XP_ExplorerTreeViewImageSet();

		public virtual string Collapse => HasImages_Collapse ? FormatFilename("Collapse") : "";
		public virtual string Expand => HasImages_Collapse ? FormatFilename("Expand") : "";
		public virtual string NoExpand  => HasImages_Collapse ? FormatFilename("NoExpand") : "";
		public virtual string LeafNode => FormatFilename("LeafNode"); 
		public virtual string RootNode => FormatFilename("RootNode");
		public virtual string ParentNode => FormatFilename("ParentNode");

		public string FormatFilename(string imageFilebase) =>
			string.Concat(this.GetType().Name.Replace("TreeViewImageSet", ""), "_", imageFilebase, ".gif");

		public virtual bool HasImages_Collapse { get; protected set; } = true;
		public virtual bool HasImages_Node { get; protected set; } = true;

	}

	public class ArrowsTreeViewImageSet : TreeViewImageSet { }
	public class BulletedListTreeViewImageSet : TreeViewImageSet { }
	public class BulletedList2TreeViewImageSet : TreeViewImageSet { }
	public class BulletedList3TreeViewImageSet : TreeViewImageSet { }
	public class BulletedList4TreeViewImageSet : TreeViewImageSet { }
	public class ContactsTreeViewImageSet : TreeViewImageSet { }
	public class DefaultTreeViewImageSet : TreeViewImageSet { }
	public class EventsTreeViewImageSet : TreeViewImageSet { }
	public class FAQTreeViewImageSet : TreeViewImageSet { }
	public class InboxTreeViewImageSet : TreeViewImageSet { }
	public class MSDNTreeViewImageSet : TreeViewImageSet { }
	public class NewsTreeViewImageSet : TreeViewImageSet { }
	public class SimpleTreeViewImageSet : TreeViewImageSet { }
	public class Simple2TreeViewImageSet : TreeViewImageSet { }
	public class WindowsHelpTreeViewImageSet : TreeViewImageSet { }
	public class XP_ExplorerTreeViewImageSet : TreeViewImageSet { }

}
