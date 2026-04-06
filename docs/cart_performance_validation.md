# Cart Performance + Regression Validation

## Scope
- Step 7 validation for card-order auto-sync and navigation-only CTA flow.
- Verifies performance characteristics and functional regressions for card and swipe behavior.

## Before vs After (Measured/Derived)
- Before migration: card submit path performed per-item cart writes, so submit latency grew with item count.
- After migration: card selection changes are batched and sync writes are consolidated; navigation triggers a final flush only when needed.
- Operation count evidence: `CardFlow_BurstChangesThenFlush_UsesSingleBatchWriteEvenForLargeSelectionCounts` validates one batch write (and one read) for a 100/100/100 entree/side/drink draft.

## Validation Checklist
- [x] Click-to-navigation is no longer proportional to item count.
- Evidence: navigation path no longer loops per item; final flush invokes one batch update call.
- [x] No extra menu API calls occur during card-order submit/navigation.
- Evidence: card CTA path flushes existing in-memory draft to cart and navigates; no menu-service fetch in submit path.
- [x] Swipe-order flow still works end-to-end.
- Evidence: swipe path remains on `CartSubmitter.SubmitAsync` and existing submitter tests continue to pass.

## Tests Used
- `CardOrderFlowTests`
- `CardOrderAutoSyncCoordinatorTests`
- `CardOrderDraftManagerTests`
- `StationDraftToOrderMapperTests`
- `CartServiceTests`
- `CartSubmitterTests`

## Notes
- Large card orders can exceed default SignalR payload constraints on Blazor Server; `MaximumReceiveMessageSize` has been increased to support large cart synchronization payloads.
