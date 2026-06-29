#!/usr/bin/env node
/* eslint-disable no-console */
/**
 * Sonar Security Agent
 *
 * Calls the SonarQube API to retrieve the Quality Gate state and writes a markdown
 * summary plus a JSON report.
 *
 * Usage:
 *   SONAR_TOKEN=... SONAR_HOST_URL=https://sonar.example.com node sonar-security-agent.mjs
 */
import { writeFileSync } from 'node:fs';
import { exit } from 'node:process';

const sonarToken = process.env.SONAR_TOKEN;
const sonarHost = process.env.SONAR_HOST_URL;
const projectKey = 'EmployeeManagementPortal';

if (!sonarToken || !sonarHost) {
  console.error('SONAR_TOKEN and SONAR_HOST_URL must be set.');
  exit(1);
}

const auth = Buffer.from(`${sonarToken}:`).toString('base64');
const response = await fetch(`${sonarHost}/api/qualitygates/project_status?projectKey=${projectKey}`, {
  headers: { Authorization: `Basic ${auth}` }
});

if (!response.ok) {
  console.error(`Sonar API responded ${response.status}: ${await response.text()}`);
  exit(1);
}

const data = await response.json();
const status = data.projectStatus;

const summary = {
  generatedAt: new Date().toISOString(),
  qualityGate: status.status,
  conditions: status.conditions.map(c => ({
    metric: c.metricKey,
    status: c.status,
    threshold: c.errorThreshold,
    actual: c.actualValue
  })),
  reliability: status.conditions.find(c => c.metricKey === 'reliability_rating')?.actualValue ?? 'N/A',
  security: status.conditions.find(c => c.metricKey === 'security_rating')?.actualValue ?? 'N/A',
  maintainability: status.conditions.find(c => c.metricKey === 'sqale_rating')?.actualValue ?? 'N/A',
  coverage: status.conditions.find(c => c.metricKey === 'coverage')?.actualValue ?? 'N/A',
  duplications: status.conditions.find(c => c.metricKey === 'duplicated_lines_density')?.actualValue ?? 'N/A'
};

writeFileSync('security-summary.json', JSON.stringify(summary, null, 2));

const md = [
  '# SonarQube Security Summary',
  '',
  `**Generated**: ${summary.generatedAt}`,
  '',
  `## Quality Gate: ${status.status}`,
  '',
  '| Metric | Status | Threshold | Value |',
  '|---|---|---|---|',
  ...summary.conditions.map(c => `| ${c.metric} | ${c.status} | ${c.threshold} | ${c.actual} |`),
  '',
  '## Ratings',
  '',
  `- Reliability: ${summary.reliability}`,
  `- Security: ${summary.security}`,
  `- Maintainability: ${summary.maintainability}`,
  `- Coverage: ${summary.coverage}%`,
  `- Duplications: ${summary.duplications}%`
].join('\n');

writeFileSync('security-summary.md', md);
console.log(`Quality gate: ${status.status}`);
if (status.status !== 'OK') exit(1);
