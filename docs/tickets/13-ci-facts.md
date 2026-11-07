---
title: "CI: current facts for the workflow, test reporting and rulesets"
type: research
for: 02-ci
status: closed
blocked_by: []
---

## Question
Facts the CI choices wait on, as of the research date. The repo is `NawarAli1912/SlotDrop` (public, personal account, created 2026-11-05); tests run on Microsoft Testing Platform with xunit.v3 4.0.2 ([Skeleton tool facts](09-skeleton-tool-facts.md) §3).
- Test reporting options and their limits on a public repo:
  - `dorny/test-reporter`: current version; does `reporter: dotnet-trx` parse TRX from xunit.v3 `--report-xunit-trx` and from MTP `--report-trx` (`Microsoft.Testing.Extensions.TrxReport`)? Compare a locally generated TRX against what its parser reads.
  - Built-in alternatives: does xunit.v3 or MTP have a GitHub Actions reporter or a job-summary output (`$GITHUB_STEP_SUMMARY`)?
  - Uploading the TRX as an artifact (`actions/upload-artifact` version, retention).
- Action versions: `actions/checkout`, `actions/setup-dotnet` (caching with several `packages.lock.json` and `cache-dependency-path`), `actions/upload-artifact`.
- Workflow details: `concurrency` with `cancel-in-progress` for PRs; minimal `permissions`; whether `checks: write` is needed per reporting option.
- Rulesets on a public personal repo (free plan): which rules exist (require PR, required approvals 0 for a solo dev, required status checks with the GitHub Actions integration as source, strict up-to-date, allowed merge methods = squash, block force push/deletion), and the exact `gh api` call to create one. Who can bypass by default, and whether the owner bypasses.
- Repo merge settings: squash-only and auto-delete head branches via `gh repo edit` or `gh api`.

## Resolution
_Researched 2026-11-05._

Local evidence below comes from a copy of the repo in the session scratchpad (`research-13/repo`), with a temporary `FailingTests.cs` (one failing `Assert.Equal(1, 2)`, one `[Fact(Skip=…)]`) added only there. SDK 10.0.403, xunit.v3 4.0.2, MTP 2.5.1 (from `tests/SlotDrop.ArchitectureTests/packages.lock.json`).

### Action versions
- `actions/checkout` latest is v7.0.1 (2026-07-20); the `v7` tag points to the same commit `3d3c42e`. `gh api repos/actions/checkout/releases/latest` → `v7.0.1 2026-07-20T15:10:05Z`
- `actions/setup-dotnet` latest is v6.1.0 (2026-11-03); `v6` = `v6.1.0` = `0f4a354`. `gh api repos/actions/setup-dotnet/releases/latest` → `v6.1.0 2026-11-03T02:51:23Z`
- `actions/upload-artifact` latest is v7.0.2 (2026-10-07); `v7` = `v7.0.2` = `cf430e0`. `gh api repos/actions/upload-artifact/releases/latest` → `v7.0.2 2026-10-07T10:08:14Z`
- `dorny/test-reporter`: the newest tag is v3.2.0 (commit date 2026-10-03), and `v3` points to it (`d1b5dff`). The newest GitHub *Release* object is still v3.0.0, so `releases/latest` under-reports. `gh api repos/dorny/test-reporter/tags` → `v3.2.0 v3.1.0 v3.0.0 v3 v2.7.0`; `gh api repos/dorny/test-reporter/commits/v3 --jq .sha` = `commits/v3.2.0` = `d1b5dff…`
- All four actions run on `node24` (`runs.using` in each `action.yml` at those tags). https://github.com/dorny/test-reporter/blob/v3.2.0/action.yml, https://github.com/actions/setup-dotnet/blob/v6.1.0/action.yml, https://github.com/actions/upload-artifact/blob/v7.0.2/action.yml
- `actions/checkout` v7 refuses to check out PR code under `pull_request_target` unless you set `allow-unsafe-pr-checkout: true`. It also stores `persist-credentials` in a file under `$RUNNER_TEMP`, not in `.git/config`. https://github.com/actions/checkout/blob/v7.0.1/README.md

### setup-dotnet caching
- `cache` is off by default. When `cache-dependency-path` is not set, the action looks for `packages.lock.json` **in the repo root only** and throws if none is there. SlotDrop has no root lock file (5 lock files under `src/*` and `tests/*`), so `cache: true` alone would fail. https://github.com/actions/setup-dotnet/blob/v6.1.0/src/cache-restore.ts (`findLockFile`), README "Caching NuGet packages"
- `cache-dependency-path` "Supports wildcards or a list of file names". The key is `glob.hashFiles(path)`, so `cache-dependency-path: '**/packages.lock.json'` covers all five. https://github.com/actions/setup-dotnet/blob/v6.1.0/action.yml, `src/cache-restore.ts`
- The action caches only the NuGet `global-packages` folder. The README pairs caching with `dotnet restore --locked-mode`, and its monorepo example sets `NUGET_PACKAGES: ${{ github.workspace }}/.nuget/packages`. https://github.com/actions/setup-dotnet/blob/v6.1.0/README.md
- `global-json-file` reads the SDK version from `global.json`. With `rollForward: latestFeature`, `sdk.version` must be a full version (ours is `10.0.403`). https://github.com/actions/setup-dotnet/blob/v6.1.0/README.md ("Using the global-json-file input")
- The cache is saved in a post step with `post-if: success()`, so a failed job saves no cache. https://github.com/actions/setup-dotnet/blob/v6.1.0/action.yml

### TRX generation (local)
- xunit.v3 4.0.2 on MTP produces TRX natively with `--report-xunit-trx` (and `--report-xunit-trx-filename`), with no extra package. `dotnet test --solution SlotDrop.slnx --help` lists `--report-xunit-trx`, plus `-ctrf`, `-html`, `-junit`, `-markdown`, `-nunit` and `-xml`.
- Without `Microsoft.Testing.Extensions.TrxReport`, `--report-trx` does not exist (it is missing from the same `--help` output). xunit.v3 pulls in only `Microsoft.Testing.Extensions.TrxReport.Abstractions` 2.5.1 (packages.lock.json). https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-test-reports#visual-studio-test-reports-trx
- After `dotnet add tests/SlotDrop.ArchitectureTests package Microsoft.Testing.Extensions.TrxReport --version 2.5.1`, one run with both switches wrote two files. `dotnet test --solution SlotDrop.slnx --report-xunit-trx --report-xunit-trx-filename xunit.trx --report-trx --report-trx-filename mtp.trx` → `TestResults/xunit.trx`, `TestResults/mtp.trx` (results go to `<repo>/TestResults` by default). The NuGet latest is 2.5.2. https://api.nuget.org/v3-flatcontainer/microsoft.testing.extensions.trxreport/index.json
- Both TRX files use namespace `http://microsoft.com/schemas/VisualStudio/TeamTest/2010` and contain `Times@start/finish`, `TestDefinitions/UnitTest@id` with `TestMethod@className`, and `Results/UnitTestResult@testId/testName/duration/outcome`. Failed results add `Output/ErrorInfo/Message` and `StackTrace` (`… in /…/FailingTests.cs:line 6`), and skips are `outcome="NotExecuted"`. The local files are in the scratchpad at `research-13/trx-passing/` and `research-13/trx-failing/`.
- Differences: MTP's TRX starts with a UTF-8 BOM (`efbbbf`) and xunit's does not. MTP's `Times` use `Z` (`2026-11-05T21:02:29.571769Z`), xunit's use `+00:00` with 7 fractional digits. MTP adds `ResultSummary/RunInfos/RunInfo outcome="Error"` ("Exit code indicates failure: '2'") on a failing run. `ResultSummary@outcome` is `Completed` for a passing run (skips included) and `Failed` for a failing run, in both files.

### dorny/test-reporter `reporter: dotnet-trx` vs these TRX files
- Fields the parser reads (v3.2.0 `src/parsers/dotnet-trx/dotnet-trx-parser.ts`): `TestRun.Times[0].$.start/finish` (regex `^\d{4}-…T…\.\d+([+-]hh:mm|Z)$`), `TestDefinitions[].UnitTest[].$.id`, `UnitTest.TestMethod[0].$.className`, `Results[].UnitTestResult[].$.testId/testName/duration/outcome` (duration regex `^(\d\d):(\d\d):(\d\d(?:\.\d+)?)$`), `Output[0].ErrorInfo[0].Message[0]/StackTrace[0]`, and the source line via `/ in (.+):line (\d+)$/`. Since v3.2.0 it also reads `ResultSummary.$.outcome` and `RunInfos.RunInfo`. https://github.com/dorny/test-reporter/blob/v3.2.0/src/parsers/dotnet-trx/dotnet-trx-parser.ts, `src/utils/parse-utils.ts`
- All four local files (xunit and MTP, passing and failing) meet every one of those requirements. I checked them statically with a Python script that reproduces the parser's lookups and regexes (dorny's code was not run): Times ISO OK, every `testId` resolves, durations parse, `testName` is prefixed by `className.`, failed results have Message and StackTrace, and the stack trace matches ` in <file>:line N`. `python3 -I scratchpad/research-13/scripts/check_trx.py …` → all `OK`.
- The MTP BOM is handled. xml2js runs `bom.stripBOM` before parsing (https://github.com/Leonidas-from-XIV/node-xml2js/blob/master/src/parser.coffee), and v3.1.0 added "Decode report files before parsing and handle byte-order marks correctly". https://github.com/dorny/test-reporter/blob/v3.2.0/CHANGELOG.md
- v3.2.0 (#825) maps unknown outcomes to failed and adds a synthetic failed "Test run" entry when `ResultSummary@outcome` is not `Completed`/`Passed`/`Warning` and no test failed. Our failing runs have failing tests, so that entry is not added. Passing runs are `Completed`, so nothing changes. https://github.com/dorny/test-reporter/blob/v3.2.0/CHANGELOG.md
- The README lists dotnet-trx support for xUnit, NUnit and MSTest, documented with the VSTest `--logger trx`. It does not mention MTP or xunit.v3 by name. https://github.com/dorny/test-reporter/blob/v3.2.0/README.md
- Unverified: an end-to-end run of dorny on these files in Actions. The parse compatibility above is static only.
- `use-actions-summary` defaults to `'true'` (v3.2.0 `action.yml`). In that mode `main.ts` only writes `core.summary` (the job summary). It does not call `checks.create`/`checks.update` and produces no annotations. The check run and annotations are created only when `use-actions-summary: 'false'`. https://github.com/dorny/test-reporter/blob/v3.2.0/src/main.ts
- With `fail-on-error: 'true'` (the default) and failed tests, the step calls `core.setFailed`, in either mode. `src/main.ts`
- The README example still declares `permissions: contents: read, actions: read, checks: write`. It says fork PRs get a read-only token and "can't create check runs", and recommends a two-workflow split (artifact + `workflow_run`) for public repos. https://github.com/dorny/test-reporter/blob/v3.2.0/README.md
- From the code, `checks: write` is needed only for `use-actions-summary: 'false'`, and `actions: read` only for the `artifact:` input (the ArtifactProvider uses the token). Unverified: whether the summary-only mode works with `contents: read` alone. This is inferred from the code, not tested.
- Limits: a report is at most 65535 bytes, and `list-suites`/`list-tests` are trimmed automatically. https://github.com/dorny/test-reporter/blob/v3.2.0/README.md ("GitHub limitations")
- Open issues touch TRX paths and summaries (#532 finding `.trx` paths, #654 trimmed report fails to write to the action summary, #675 summary broken with multiple artifacts). None of them is about MTP or xunit.v3 TRX. `gh search issues --repo dorny/test-reporter trx`

### Built-in summary reporters (no third-party action)
- xunit.v3 has **no GitHub Actions reporter**. Its built-in runner reporters are AppVeyor, Default, Json, Quiet, Silent, TeamCity, Verbose and Vsts. `gh api "repos/xunit/xunit/git/trees/main?recursive=1"` → `src/xunit.v3.runner.common/Reporters/Builtin/*`. With `GITHUB_ACTIONS=true` and `GITHUB_STEP_SUMMARY` set, a plain `dotnet test` wrote nothing to the summary file (local run).
- xunit.v3 4.0.2 has `--report-xunit-markdown [--report-xunit-markdown-filename]`. It writes a Markdown results file (totals, then failures with message and stack trace, then skips) into `TestResults/`. A workflow step could append it with `cat TestResults/<name>.md >> "$GITHUB_STEP_SUMMARY"`. Local `dotnet test … --report-xunit-markdown --report-xunit-markdown-filename summary.md` produced `### Test Results … ❌ 1 failed … #### Failed ❌`. The xunit.net MTP page does not list this option yet: https://xunit.net/docs/getting-started/v3/microsoft-testing-platform
- MTP has a first-party GitHub Actions reporter in package `Microsoft.Testing.Extensions.GitHubActionsReport` (MTP 2.3.0+, non-experimental since 2.4.0). Its switch is `--report-gh`, and it activates only when `GITHUB_ACTIONS=true`. It provides per-assembly log groups, annotations for failed and skipped tests, a Markdown job summary appended to `$GITHUB_STEP_SUMMARY`, and slow-test notices. https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-test-reports#github-actions-reports
- `--report-gh` options: `--report-gh-groups on|off`, `--report-gh-annotations on|off`, `--report-gh-step-summary on|off|on-failure` (on-failure needs 2.4.0+), `--report-gh-step-summary-sections test-results|slow-tests|coverage|all`, `--report-gh-failure-details on|off`, `--report-gh-history <file>` with `--report-gh-history-window`, `--report-gh-slow-test-notices`, and `--report-gh-slow-test-threshold`. Same URL.
- NuGet versions of `Microsoft.Testing.Extensions.GitHubActionsReport`: 2.4.0, 2.4.1, 2.5.0, 2.5.1, 2.5.2 (latest). https://api.nuget.org/v3-flatcontainer/microsoft.testing.extensions.githubactionsreport/index.json
- Local test: with 2.5.1 added (matching the MTP 2.5.1 that xunit.v3 4.0.2 uses), `GITHUB_ACTIONS=true GITHUB_STEP_SUMMARY=<file> dotnet test --solution SlotDrop.slnx --report-gh` wrote `## ❌ Test Run Summary — SlotDrop.ArchitectureTests (net10.0)`. It contained a totals table (Total/Passed/Failed/Skipped/Flaky/Duration), a `<details>` block per failure with exception type, `Location: tests/SlotDrop.ArchitectureTests/FailingTests.cs:6`, message and stack, and "Slowest tests".
- Unverified: whether `::error`/`::group` workflow commands reach the Actions log. No `::` lines appeared in local `dotnet test` stdout (nor with `--output detailed`), so annotations need checking on a real runner.
- The GitHub reporter needs no token permissions. It only writes workflow commands and the step-summary file, so it needs no `checks: write` and works the same on fork PRs. This is inferred from the docs description above, which mentions no API calls.
- Note: `GitHubActionsTestLogger` (a third-party package) uses `--report-github`, a different option and not an alias of `--report-gh`. https://learn.microsoft.com/dotnet/core/testing/migrating-vstest-microsoft-testing-platform
- MTP 2.4.0+ (experimental) merges TRX/JUnit/CTRF/HTML from multi-module runs into `TestResults/merged/`. Report filenames support `{asm}`, `{tfm}`, `{arch}`, `{pname}`, `{pid}`, `{time}`, and the MTP TRX default is `{asm}_{tfm}_{arch}.trx`. https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-test-reports

### `$GITHUB_STEP_SUMMARY`
- A step's summary is limited to 1 MiB. If it is larger, the upload fails with an error annotation but the step and job status are unaffected. At most 20 step summaries are shown per job. Steps are isolated, secrets are masked automatically, `>` overwrites a summary and deleting the file removes it. https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-commands#adding-a-job-summary

### upload-artifact
- `actions/upload-artifact@v7` inputs: `name` (default `artifact`), `path`, `if-no-files-found` (`warn` default, or `error`/`ignore`), `retention-days`, `compression-level`, `overwrite`, `include-hidden-files`, and `archive` (default `true`; `false` uploads a single file unzipped and uses its filename as the artifact name). https://github.com/actions/upload-artifact/blob/v7.0.2/action.yml
- `retention-days` must be between 1 and 90 "unless changed from the repository settings page", and `0` uses the default retention. Artifacts are kept for 90 days by default. https://github.com/actions/upload-artifact/blob/v7.0.2/README.md#retention-period
- For public repos, the retention setting can be 1–90 days (400 for private), with a default of 90. https://docs.github.com/en/organizations/managing-organization-settings/configuring-the-retention-period-for-github-actions-artifacts-and-logs-in-your-organization
- Artifacts from v4 onward are immutable: the same name cannot be uploaded twice in a run unless you set `overwrite: true`. https://github.com/actions/upload-artifact/blob/v7.0.2/README.md
- Unverified: whether upload-artifact needs any `permissions:` entry. Its README documents none, and it authenticates with the runner's runtime token, not `GITHUB_TOKEN`.

### Workflow details
- An example on the concurrency page cancels in-progress runs only for PRs: `concurrency: { group: ${{ github.head_ref || github.run_id }}, cancel-in-progress: true }`. `head_ref` is set only on PR events, so pushes get a unique group. `cancel-in-progress` also accepts an expression, e.g. `${{ github.event_name == 'pull_request' }}`. https://docs.github.com/en/actions/how-tos/write-workflows/choose-when-workflows-run/control-workflow-concurrency
- A concurrency group holds at most one pending run, and a newly queued run cancels the older pending one. `queue: max` allows 100 pending runs but cannot be combined with `cancel-in-progress: true`. Same URL.
- Setting any key under `permissions:` sets every unlisted permission to `none`. Keys include `actions`, `checks`, `contents`, `pull-requests` and `statuses`. https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax#permissions
- For `pull_request` events from forks, write permissions become read-only unless the repo enables "Send write tokens to workflows from pull requests". Same URL.
- Permissions needed per reporting option, from the evidence above:
  - MTP `--report-gh`: `contents: read`.
  - xunit markdown appended to the summary: `contents: read`.
  - dorny with `use-actions-summary: 'true'`: `contents: read` (unverified, see above).
  - dorny check-run mode: `checks: write`, and it does not work on fork PRs without a `workflow_run` split.
  - upload-artifact: no token scope documented.

### Rulesets (public personal repo, GitHub Free)
- Rulesets are available "in public repositories with GitHub Free". The repo has none yet, and no classic branch protection. https://github.com/github/docs/blob/main/content/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets.md; `gh api repos/NawarAli1912/SlotDrop/rulesets` → `[]`; `gh api repos/NawarAli1912/SlotDrop/branches/main/protection` → `404 Branch not protected`
- Create endpoint: `POST /repos/{owner}/{repo}/rulesets`, with recommended header `X-GitHub-Api-Version: 2026-03-10`. Body: `name` (required), `target` (`branch` default), `enforcement` (`active`/`evaluate`/`disabled`, required), `bypass_actors` (optional), `conditions.ref_name.include/exclude` (accepts `~DEFAULT_BRANCH`, `~ALL`, `refs/heads/...`), `rules`. https://docs.github.com/en/rest/repos/rules#create-a-repository-ruleset
- The `pull_request` rule has these required parameters: `required_approving_review_count` (0–10), `dismiss_stale_reviews_on_push`, `require_code_owner_review`, `require_last_push_approval`, `required_review_thread_resolution`. Optional: `allowed_merge_methods` (an array of `merge`/`squash`/`rebase`, where "At least one option must be enabled"), `required_reviewers`, `require_extra_approval_for_unattributed_changes`, `dismissal_restriction`. Same URL.
- The squash-only field is `rules[].parameters.allowed_merge_methods: ["squash"]` on the `pull_request` rule. If the repo settings disable the method the ruleset requires, merging is blocked. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets (require a pull request, allowed merge methods)
- With 0 approvals, a PR is still required: "The pull request doesn't necessarily have to be approved, but it must be opened". The extra approval for unattributed Copilot PRs "has no effect if the ruleset requires zero approvals". Same URL.
- `required_status_checks` takes `required_status_checks[]` objects (`context` required, `integration_id` optional), `strict_required_status_checks_policy` (required; `true` means the branch must be up to date), and optional `do_not_enforce_on_create`. https://docs.github.com/en/rest/repos/rules#create-a-repository-ruleset
- The GitHub Actions `integration_id` is **15368**, the id of the `github-actions` GitHub App. `gh api apps/github-actions --jq '{id,slug,name}'` → `{"id":15368,"name":"GitHub Actions","slug":"github-actions"}` (endpoint: https://docs.github.com/en/rest/apps/apps#get-an-app)
- Docs say an app chosen as the source "must have recently submitted a check run" and be installed with `statuses:write`. Unverified: whether the API rejects `integration_id: 15368` before the first workflow run. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets#require-status-checks-to-pass-before-merging
- For a workflow, the status-check `context` is the job name, `<job name>`. For a reusable workflow it is `<job name> / <reusable job name>`. Matrix, event and workflow are not considered. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/troubleshooting-rules#troubleshooting-required-status-checks
- `non_fast_forward` (blocks force pushes) and `deletion` take only `type`. https://docs.github.com/en/rest/repos/rules#create-a-repository-ruleset
- Exact call. Replace `build-test` with the job's `name:` (or its id if it has no name):
  ```sh
  gh api --method POST -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2026-03-10" \
    repos/NawarAli1912/SlotDrop/rulesets --input - <<'JSON'
  {
    "name": "main",
    "target": "branch",
    "enforcement": "active",
    "bypass_actors": [],
    "conditions": { "ref_name": { "include": ["refs/heads/main"], "exclude": [] } },
    "rules": [
      { "type": "deletion" },
      { "type": "non_fast_forward" },
      { "type": "pull_request", "parameters": {
          "required_approving_review_count": 0,
          "dismiss_stale_reviews_on_push": false,
          "require_code_owner_review": false,
          "require_last_push_approval": false,
          "required_review_thread_resolution": false,
          "allowed_merge_methods": ["squash"] } },
      { "type": "required_status_checks", "parameters": {
          "strict_required_status_checks_policy": true,
          "required_status_checks": [ { "context": "build-test", "integration_id": 15368 } ] } }
    ]
  }
  JSON
  ```
- Default bypass: `bypass_actors` is optional and starts empty, and the docs grant bypass only to listed actors ("only users with bypass permissions can delete…"). So by default **nobody bypasses, including the repo owner and admin**. The owner must also go through a PR with green checks. https://docs.github.com/en/rest/repos/rules#create-a-repository-ruleset, https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets#restrict-deletions
- Unverified: whether GitHub shows an implicit admin bypass in the PR merge box for personal repos with an empty bypass list. The docs do not state it.
- Bypass-eligible actors are repository admins (plus org and enterprise owners), maintain/write roles, teams, GitHub Apps and Dependabot. `bypass_mode` is `always` (default), `pull_request` ("For pull requests only") or `exempt`. `OrganizationAdmin` "is not applicable for personal repositories". https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/creating-rulesets-for-a-repository#granting-bypass-permissions-for-your-branch-or-tag-ruleset, https://docs.github.com/en/rest/repos/rules
- Unverified: the `actor_id` for the Repository admin role. It is commonly `{"actor_type":"RepositoryRole","actor_id":5}`, but the REST docs do not list role ids.
- A ruleset GET response includes `current_user_can_bypass` (`always`/`pull_requests_only`/`never`/`exempt`), which shows whether the owner can bypass once the ruleset exists. https://docs.github.com/en/rest/repos/rules

### Repo merge settings
- Current settings: `allow_squash_merge: true`, `allow_merge_commit: true`, `allow_rebase_merge: true`, `delete_branch_on_merge: false`, `allow_auto_merge: false`, `squash_merge_commit_title: COMMIT_OR_PR_TITLE`, `squash_merge_commit_message: COMMIT_MESSAGES`. `gh api repos/NawarAli1912/SlotDrop`
- `gh` 2.94.0 flags: `--enable-squash-merge`, `--enable-merge-commit`, `--enable-rebase-merge`, `--delete-branch-on-merge`, `--enable-auto-merge`, and `--squash-merge-commit-message {default|pr-title|pr-title-commits|pr-title-description}`. Squash-only plus head-branch deletion is `gh repo edit NawarAli1912/SlotDrop --enable-squash-merge --enable-merge-commit=false --enable-rebase-merge=false --delete-branch-on-merge`. `gh repo edit --help`
- The REST equivalent is `gh api --method PATCH repos/NawarAli1912/SlotDrop -F allow_squash_merge=true -F allow_merge_commit=false -F allow_rebase_merge=false -F delete_branch_on_merge=true`. These are the same fields the GET returns. https://docs.github.com/en/rest/repos/repos#update-a-repository
