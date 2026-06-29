#!/usr/bin/env bash
# scripts/run-tests.sh
# Restore + build + run all tests with coverage; emit cobertura and opencover xmls at the repo root.
set -euo pipefail

SOLUTION="${SOLUTION:-EmployeeManagementPortal.sln}"
TEST_PROJECT="${TEST_PROJECT:-tests/EmployeeManagementPortal.Tests/EmployeeManagementPortal.Tests.csproj}"
COVERAGE_DIR="${COVERAGE_DIR:-coverage}"

echo "==> Restoring $SOLUTION"
dotnet restore "$SOLUTION"

echo "==> Building $SOLUTION (Debug)"
dotnet build "$SOLUTION" --no-restore --configuration Debug

echo "==> Running tests with coverage"
rm -rf "$COVERAGE_DIR"
dotnet test "$TEST_PROJECT" \
    --no-build \
    --configuration Debug \
    --results-directory "$COVERAGE_DIR" \
    --collect:"XPlat Code Coverage" \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover \
    --logger "trx;LogFileName=test-results.trx"

# Flatten the generated xml files so downstream tools (Sonar, CodeQL scripts) can find them by name.
COBERTURA=$(find "$COVERAGE_DIR" -name "coverage.cobertura.xml" | head -n 1)
OPENCOVER=$(find "$COVERAGE_DIR" -name "coverage.opencover.xml" | head -n 1)
[ -f "$COBERTURA" ] && cp "$COBERTURA" coverage.cobertura.xml
[ -f "$OPENCOVER" ] && cp "$OPENCOVER" coverage.opencover.xml

echo "==> Coverage artifacts:"
ls -la coverage*.xml || true