### ItemType renames must cover all consumers (tests, samples, docs)

**By:** Cyclops
**What:** When renaming a generic type parameter on a component (e.g., `TItem` → `ItemType`), the rename must be applied to all consumer files — test `.razor` files, sample pages, and documentation code blocks — not just the component source. CI may only report the first few errors, masking the full scope.
**Why:** The `ItemType` standardization renamed the generic on 13+ components but missed 43 consumer files. This broke CI on PR #425 with `RZ10001` and `CS0411` errors across RadioButtonList, BulletedList, CheckBoxList, DropDownList, ListBox tests and all related sample pages.
