#!/usr/bin/env node
/* eslint-disable no-console */
/**
 * Security Analysis Agent
 *
 * Reads SARIF files produced by CodeQL, classifies findings by severity,
 * maps them to OWASP / CWE, computes a risk score, and writes reports.
 *
 * Usage:
 *   node security-analysis-agent.mjs <sarif-dir-glob>
 */
import { readdirSync, readFileSync, writeFileSync, existsSync, statSync } from 'node:fs';
import { join } from 'node:path';
import { argv, exit } from 'node:process';

const inputDir = argv[2] ?? '.';
const sarifFiles = [];
if (existsSync(inputDir)) {
  for (const entry of readdirSync(inputDir)) {
    const full = join(inputDir, entry);
    if (entry.endsWith('.sarif') && statSync(full).isFile()) sarifFiles.push(full);
  }
}

const counts = { error: 0, warning: 0, note: 0 };
const cweMap = new Map();
const owaspMap = new Map();
const findings = [];

for (const file of sarifFiles) {
  const sarif = JSON.parse(readFileSync(file, 'utf-8'));
  for (const run of sarif.runs || []) {
    for (const rule of run.tool?.driver?.rules ?? []) {
      const tags = rule.properties?.tags ?? [];
      const cwe = tags.find(t => t.startsWith('cwe-')) ?? 'cwe-unknown';
      const owasp = tags.find(t => t.startsWith('owasp-')) ?? 'owasp-unmapped';
      cweMap.set(cwe, (cweMap.get(cwe) ?? 0) + 1);
      owaspMap.set(owasp, (owaspMap.get(owasp) ?? 0) + 1);
    }
    for (const r of run.results || []) {
      const level = r.level ?? 'warning';
      counts[level] = (counts[level] ?? 0) + 1;
      findings.push({
        ruleId: r.ruleId,
        level,
        message: r.message?.text ?? '',
        file: r.locations?.[0]?.physicalLocation?.artifactLocation?.uri ?? '',
        line: r.locations?.[0]?.physicalLocation?.region?.startLine ?? 0
      });
    }
  }
}

const riskScore = counts.error * 10 + counts.warning * 3 + counts.note;

const summary = {
  generatedAt: new Date().toISOString(),
  totals: counts,
  riskScore,
  cwe: Object.fromEntries(cweMap),
  owasp: Object.fromEntries(owaspMap),
  findings: findings.slice(0, 50)
};

writeFileSync('security-analysis.json', JSON.stringify(summary, null, 2));

const md = [
  '# CodeQL Security Analysis',
  '',
  `**Generated**: ${summary.generatedAt}`,
  '',
  '## Findings',
  '',
  '| Severity | Count |',
  '|---|---|',
  `| Critical / Error | ${counts.error} |`,
  `| Medium / Warning | ${counts.warning} |`,
  `| Low / Note | ${counts.note} |`,
  '',
  `**Risk Score**: ${riskScore}`,
  '',
  '## OWASP Mapping',
  '',
  ...[...owaspMap.entries()].map(([k, v]) => `- **${k}** — ${v} findings`),
  '',
  '## CWE Mapping',
  '',
  ...[...cweMap.entries()].slice(0, 10).map(([k, v]) => `- **${k}** — ${v} findings`),
  '',
  '## Recommendations',
  '',
  '- Remediate all critical findings before release.',
  '- Review warnings and either fix or document the suppression rationale.',
  '- Re-run CodeQL after remediation to confirm zero new regressions.'
].join('\n');

writeFileSync('security-analysis.md', md);
console.log(`Risk score: ${riskScore}`);
