#!/usr/bin/env python3
"""
Generate performance charts for the BWFC Migration Toolkit Executive Summary.

Data sources:
  - dev-docs/migration-tests/wingtiptoys/run01–run17 REPORT.md files
  - dev-docs/migration-tests/contosouniversity/run01–run18 REPORT.md files

Output:
  - wingtiptoys-layer1-perf.png
  - contosouniversity-layer1-perf.png
  - combined-improvement.png

Usage:
  python generate-charts.py
"""

import os
import numpy as np
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.ticker import MultipleLocator

# ── Shared style ────────────────────────────────────────────────────────────

plt.rcParams.update({
    "figure.facecolor": "white",
    "axes.facecolor": "white",
    "axes.edgecolor": "#CCCCCC",
    "axes.grid": True,
    "grid.color": "#E0E0E0",
    "grid.linestyle": "--",
    "grid.linewidth": 0.6,
    "font.family": "sans-serif",
    "font.size": 11,
    "axes.titlesize": 14,
    "axes.titleweight": "bold",
    "axes.labelsize": 12,
})

BLUE = "#1976D2"
LIGHT_BLUE = "#BBDEFB"
ACCENT = "#E53935"
GREEN = "#43A047"
DARK_BLUE = "#0D47A1"
ORANGE = "#FB8C00"

OUTPUT_DIR = os.path.dirname(os.path.abspath(__file__))

# ── Data (extracted from REPORT.md files) ───────────────────────────────────

# WingtipToys Layer 1 times (seconds) — from run report tables
wt_runs   = [1,    4,    5,    6,    8,   11,   12,   13,   14,   15,   16,   17,   18]
wt_times  = [3.30, 3.30, 3.00, 4.58, 3.30, 3.30, 3.00, 3.00, 3.20, 2.83, 2.50, 1.81, 1.51]
wt_best_idx = wt_times.index(min(wt_times))

# ContosoUniversity Layer 1 times (seconds) — from run report tables
cu_runs   = [1,    2,    4,    6,    8,    9,   12,   15,   16,   17]
cu_times  = [1.50, 0.63, 1.00, 1.43, 1.00, 2.30, 1.00, 0.75, 1.00, 0.59]
cu_best_idx = cu_times.index(min(cu_times))


def chart_layer1(runs, times, best_idx, title, filename, y_max=None):
    """Line chart with fill, trend line, and best-time callout."""
    fig, ax = plt.subplots(figsize=(800 / 100, 400 / 100), dpi=150)

    x = np.arange(len(runs))
    labels = [f"R{r}" for r in runs]

    # Fill area under curve
    ax.fill_between(x, times, alpha=0.18, color=BLUE)

    # Main line + markers
    ax.plot(x, times, color=BLUE, linewidth=2.2, marker="o",
            markersize=7, markerfacecolor="white", markeredgecolor=BLUE,
            markeredgewidth=2, zorder=5)

    # Highlight best time
    ax.plot(x[best_idx], times[best_idx], marker="*", markersize=16,
            color=ACCENT, zorder=6)
    ax.annotate(
        f"  {times[best_idx]:.2f}s ★ Best",
        xy=(x[best_idx], times[best_idx]),
        fontsize=10, fontweight="bold", color=ACCENT,
        xytext=(12, 8), textcoords="offset points",
        arrowprops=dict(arrowstyle="-", color=ACCENT, lw=0.8),
    )

    # Annotate first run
    ax.annotate(
        f"{times[0]:.2f}s",
        xy=(x[0], times[0]),
        fontsize=9, color="#555555",
        xytext=(-8, 12), textcoords="offset points",
        ha="center",
    )

    # Trend line (linear regression)
    z = np.polyfit(x, times, 1)
    trend = np.poly1d(z)
    ax.plot(x, trend(x), color=DARK_BLUE, linewidth=1.4, linestyle="--",
            alpha=0.55, label="Trend")

    ax.set_xticks(x)
    ax.set_xticklabels(labels, fontsize=9)
    ax.set_xlabel("Run Number")
    ax.set_ylabel("Seconds")
    ax.set_title(title, pad=14)

    if y_max:
        ax.set_ylim(bottom=0, top=y_max)
    else:
        ax.set_ylim(bottom=0)

    ax.legend(loc="upper right", fontsize=9, framealpha=0.8)
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)

    fig.tight_layout()
    path = os.path.join(OUTPUT_DIR, filename)
    fig.savefig(path, dpi=150, bbox_inches="tight")
    plt.close(fig)
    print(f"  ✅ {filename} ({os.path.getsize(path):,} bytes)")


def chart_combined():
    """Grouped bar chart comparing first-run vs best-run for both projects."""
    fig, ax = plt.subplots(figsize=(800 / 100, 400 / 100), dpi=150)

    projects = ["WingtipToys", "ContosoUniversity"]
    first_times = [wt_times[0], cu_times[0]]
    best_times = [wt_times[wt_best_idx], cu_times[cu_best_idx]]
    improvements = [
        (1 - best_times[i] / first_times[i]) * 100 for i in range(2)
    ]

    x = np.arange(len(projects))
    bar_w = 0.32

    bars_first = ax.bar(x - bar_w / 2, first_times, bar_w,
                        label="First Run", color=LIGHT_BLUE,
                        edgecolor=BLUE, linewidth=1.2)
    bars_best = ax.bar(x + bar_w / 2, best_times, bar_w,
                       label="Best Run", color=BLUE,
                       edgecolor=DARK_BLUE, linewidth=1.2)

    # Value labels on bars
    for bar in bars_first:
        ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 0.06,
                f"{bar.get_height():.2f}s", ha="center", va="bottom",
                fontsize=10, color="#555555")
    for bar in bars_best:
        ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 0.06,
                f"{bar.get_height():.2f}s", ha="center", va="bottom",
                fontsize=10, fontweight="bold", color=DARK_BLUE)

    # Improvement percentage annotations
    for i, imp in enumerate(improvements):
        mid_y = (first_times[i] + best_times[i]) / 2
        ax.annotate(
            f"▼ {imp:.0f}%",
            xy=(x[i] + bar_w / 2 + 0.05, best_times[i]),
            xytext=(x[i] + bar_w + 0.18, mid_y),
            fontsize=11, fontweight="bold", color=GREEN,
            arrowprops=dict(arrowstyle="->", color=GREEN, lw=1.5),
            ha="left", va="center",
        )

    ax.set_xticks(x)
    ax.set_xticklabels(projects, fontsize=11)
    ax.set_ylabel("Seconds")
    ax.set_title("Migration Performance Improvement", pad=14)
    ax.set_ylim(bottom=0, top=max(first_times) * 1.35)
    ax.legend(loc="upper right", fontsize=10, framealpha=0.8)
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)

    fig.tight_layout()
    path = os.path.join(OUTPUT_DIR, "combined-improvement.png")
    fig.savefig(path, dpi=150, bbox_inches="tight")
    plt.close(fig)
    print(f"  ✅ combined-improvement.png ({os.path.getsize(path):,} bytes)")


if __name__ == "__main__":
    print("Generating BWFC migration performance charts …\n")

    chart_layer1(
        wt_runs, wt_times, wt_best_idx,
        "Layer 1 Execution Time — WingtipToys",
        "wingtiptoys-layer1-perf.png",
        y_max=5.0,
    )

    chart_layer1(
        cu_runs, cu_times, cu_best_idx,
        "Layer 1 Execution Time — ContosoUniversity",
        "contosouniversity-layer1-perf.png",
        y_max=2.8,
    )

    chart_combined()

    print("\nDone — 3 charts written to:", OUTPUT_DIR)
