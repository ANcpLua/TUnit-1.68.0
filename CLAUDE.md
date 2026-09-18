# TUnit showcase — version bump procedure

This repo pins one TUnit version and shows every API it covers with a runnable, asserting test.
`CHANGELOG.md` is the reverse API doc; its header explains the table schema. Follow these steps for
every bump from `OLD` to `NEW`, in one commit.

## 1. Find what changed

- Release notes for every tag in `OLD..NEW`: `gh release view vX -R thomhurst/TUnit`.
- Public API diff (the source of truth for "new API"):
  `git diff vOLD vNEW -- tests/TUnit.PublicAPI` in a clone of `thomhurst/TUnit`.
- Read each relevant PR (`gh pr view N -R thomhurst/TUnit`) and its docs diff for the exact usage.

## 2. Bump and rename

- Every `TUnit*` `PackageReference` → `NEW` (TUnit, Mocks, Playwright, AspNetCore, OpenTelemetry,
  Logging.Microsoft, and `AdvancedPatterns/AdvancedPatterns.Tests.csproj`).
- `git mv` every versioned folder, `.csproj` and the `.slnx`, inner paths first; update the `.slnx`.
- Namespaces `*_1._OLD_._0` → `*_1._NEW_._0`; paths in READMEs and comments.
- `Verified on OLD` / `pinned to TUnit OLD` / `on OLD` claims → `NEW` only after the run re-verifies them.
  `OLD:` or `Fixed in X` notes that date a change stay as they are.
- If a restore fails with NU1605, move the `Microsoft.*` package it names and say so in the CHANGELOG.

## 3. Add examples

- New public API → a runnable test that asserts the behaviour, in the project that owns the area.
- Behaviour fix → a test that fails (or does not compile) on `OLD` and passes on `NEW`. Prove it:
  point the project at `OLD`, build/run, restore `NEW`.
- `[Explicit]` for tests that must end cancelled / failed / timed out.
- Update the project README that lists its files.

## 4. Verify

- `dotnet build "TUnit NEW.slnx"` → 0 errors, 0 warnings.
- `dotnet run --no-build --project <csproj>` for all seven test projects (they are independent; run in parallel).
- Native AOT: `dotnet publish "TUnit.Patterns.Policies NEW/…csproj" -c Release -r osx-arm64 -o /tmp/aot`, run the binary.
- Re-run `[Explicit]` showcases whose claims appear in a README / the CHANGELOG with `--treenode-filter`.

## 5. CHANGELOG

- New `## [NEW] — date` section on top with the full tables (paths renamed), header lines with
  packages and per-project test counts, a `### Since OLD` list (new API, behaviour changes with the
  test that proves each, not-shown items).
- Collapse the previous version to its header lines plus
  `Superseded by the tables above; the OLD tree is commit <sha>.`

## 6. Ship

1. Commit (message lists the bump, new API, behaviour changes) and push to `origin main`.
2. `gh repo rename TUnit-NEW -R ANcpLua/TUnit-OLD --yes`.
3. `git remote set-url origin https://github.com/ANcpLua/TUnit-NEW.git`.
4. Rename the local folder `…/repo-playground/TUnit-OLD` → `TUnit-NEW` last; then any tool still
   pointing at the old path is stale.

## Rules

- Nothing in the table without a test run on the pinned version; docs are not evidence.
- `samples/`, `AspireSingleFileHost1/` and `CLAUDE.local.md` are not part of the showcase; do not commit them.
