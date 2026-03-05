# Run 7 — Layer 2/3 Timing

Layer 2 start: 2026-03-04 22:36:25
Infrastructure done: 2026-03-04 22:37:30
Pages transformed: 2026-03-04 22:38:45
Build result: PARTIAL PASS — 0 errors in core storefront (5 pages), 14 errors in out-of-scope pages (Account/Manage.razor, Checkout/CheckoutReview.razor, Admin/AdminPage.razor.cs)

## Notes

- All 14 build errors are in Account/, Checkout/, and Admin/ pages — explicitly excluded from this run's scope
- Core storefront pages (Default, ProductList, ProductDetails, ShoppingCart, MainLayout) compiled cleanly
- 13 warnings are RZ10012 (unrecognized component names) in Account/Admin pages — also out of scope
