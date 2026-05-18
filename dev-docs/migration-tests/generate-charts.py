#!/usr/bin/env python3
# Requirements: pip install matplotlib
"""Generate executive-summary SVG charts for migration benchmark reporting."""

from __future__ import annotations

from pathlib import Path

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
from matplotlib.ticker import FuncFormatter
import numpy as np

plt.style.use("seaborn-v0_8-whitegrid")

PRIMARY_BLUE = "#2563eb"
SECONDARY_ORANGE = "#f97316"
SUCCESS_GREEN = "#22c55e"
FAILURE_RED = "#ef4444"
NEUTRAL_GRAY = "#64748b"
LIGHT_BLUE = "#bfdbfe"
LIGHT_ORANGE = "#fed7aa"
OUTPUT_DIR = Path(__file__).resolve().parent / "charts"


def configure_style() -> None:
    plt.rcParams.update({
        "figure.facecolor": "white",
        "axes.facecolor": "white",
        "axes.edgecolor": "#cbd5e1",
        "axes.labelcolor": "#0f172a",
        "axes.titleweight": "bold",
        "axes.titlesize": 16,
        "axes.labelsize": 11,
        "grid.color": "#e2e8f0",
        "grid.linewidth": 0.8,
        "grid.alpha": 1.0,
        "font.family": "DejaVu Sans",
        "font.size": 10,
        "legend.frameon": False,
        "legend.fontsize": 10,
        "xtick.color": "#334155",
        "ytick.color": "#334155",
        "svg.fonttype": "none",
    })


def ensure_output_dir() -> Path:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    return OUTPUT_DIR


def seconds_to_clock(seconds: float) -> str:
    minutes = int(seconds // 60)
    remaining = int(round(seconds % 60))
    if remaining == 60:
        minutes += 1
        remaining = 0
    return f"{minutes}:{remaining:02d}" if minutes else f"{int(round(seconds))}s"


def format_seconds(value: float, _: float) -> str:
    return seconds_to_clock(value)


def finalize_axes(ax: plt.Axes) -> None:
    ax.spines["top"].set_visible(False)
    ax.spines["right"].set_visible(False)
    ax.set_axisbelow(True)


def save_figure(fig: plt.Figure, filename: str) -> None:
    path = ensure_output_dir() / filename
    fig.tight_layout()
    fig.savefig(path, format="svg", bbox_inches="tight")
    plt.close(fig)
    print(f"Wrote {path}")


def generate_error_reduction_chart() -> None:
    # Key benchmark runs with measured initial build errors
    runs =   np.array([25,  40, 70,  77, 78, 79, 80, 81, 89, 90])
    errors = np.array([382, 25,  8,  15,  1, 78, 28, 14,  2,  2])
    colors = [
        PRIMARY_BLUE, PRIMARY_BLUE, PRIMARY_BLUE, PRIMARY_BLUE,
        SUCCESS_GREEN, FAILURE_RED, SECONDARY_ORANGE, PRIMARY_BLUE,
        SUCCESS_GREEN, SUCCESS_GREEN,
    ]

    fig, ax = plt.subplots(figsize=(12, 6.2))
    bars = ax.bar(runs, errors, color=colors, width=2.5, edgecolor="white", linewidth=1.2)

    trend = np.poly1d(np.polyfit(runs, errors, 1))
    x_smooth = np.linspace(runs.min(), runs.max(), 200)
    ax.plot(x_smooth, trend(x_smooth), color=NEUTRAL_GRAY, linewidth=2.2, linestyle="--", label="Overall trend")
    ax.plot(runs, errors, color=PRIMARY_BLUE, linewidth=1.3, alpha=0.35)

    ax.annotate(
        "Run 79 regression spike\n(ComponentRef bug)",
        xy=(79, 78),
        xytext=(65, 165),
        fontsize=10,
        color="#991b1b",
        arrowprops=dict(arrowstyle="->", color=FAILURE_RED, lw=1.4),
        bbox=dict(boxstyle="round,pad=0.35", facecolor="#fef2f2", edgecolor="#fecaca"),
    )
    ax.annotate(
        "Baseline compile-surface debt",
        xy=(25, 382),
        xytext=(31.5, 335),
        fontsize=10,
        color="#1e3a8a",
        arrowprops=dict(arrowstyle="->", color=PRIMARY_BLUE, lw=1.2),
    )
    ax.annotate(
        "Run 90: 2 errors\n(identity quarantine +\nSSR form contract)",
        xy=(90, 2),
        xytext=(78, 65),
        fontsize=9,
        color="#065f46",
        arrowprops=dict(arrowstyle="->", color=SUCCESS_GREEN, lw=1.4),
        bbox=dict(boxstyle="round,pad=0.35", facecolor="#f0fdf4", edgecolor="#86efac"),
    )

    for run, bar, value in zip(runs, bars, errors):
        label = "~25" if run == 40 else str(int(value))
        ax.text(bar.get_x() + bar.get_width() / 2, value + 8, label, ha="center", va="bottom", fontsize=9, color="#0f172a")

    ax.set_title("Build Error Reduction Over Time — WingtipToys")
    ax.set_xlabel("WingtipToys benchmark run")
    ax.set_ylabel("Initial build errors")
    ax.set_xticks(runs)
    ax.set_ylim(0, 420)
    ax.legend(loc="upper right")
    finalize_axes(ax)
    save_figure(fig, "error-reduction.svg")


def generate_migration_time_chart() -> None:
    runs =       np.array([1,    40,   70,  80,  81,  88,  89,  90])
    total_time = np.array([566, 1320, 960, 600, 258, 389, 502, 384])
    l1_time =   np.array([3.3,  120,  20,  28,  26,  22,  22,  22])
    # L2 = build repair time (confirmed where available, estimated otherwise)
    l2_time =   np.array([563, 1200, 240, 238, 232, 180, 183,  61])

    fig, ax = plt.subplots(figsize=(12, 6.2))

    ax.plot(runs, total_time, marker="o", markersize=7, linewidth=2.8, color=PRIMARY_BLUE, label="Total wall-clock")
    ax.plot(runs, l2_time,   marker="o", markersize=6, linewidth=2.3, color=SECONDARY_ORANGE, label="L2 build repair")
    ax.plot(runs, l1_time,   marker="o", markersize=6, linewidth=2.3, color=SUCCESS_GREEN,    label="L1 automated migration")

    ax.fill_between(runs, total_time, color=LIGHT_BLUE, alpha=0.24)

    for x, y in zip(runs, total_time):
        ax.annotate(seconds_to_clock(float(y)), (x, y), textcoords="offset points", xytext=(0, 8), ha="center", color="#1e3a8a", fontsize=9)

    ax.annotate(
        "Run 90: 6:24 total\nL2 repair: 1:01\n(identity quarantine win)",
        xy=(90, 384),
        xytext=(76, 580),
        fontsize=9,
        color="#1e3a8a",
        arrowprops=dict(arrowstyle="->", color=PRIMARY_BLUE, lw=1.4),
        bbox=dict(boxstyle="round,pad=0.35", facecolor="#eff6ff", edgecolor="#bfdbfe"),
    )

    ax.annotate(
        "Run 40 slow path\nmore manual repair",
        xy=(40, 1320),
        xytext=(47, 1180),
        fontsize=10,
        color="#9a3412",
        arrowprops=dict(arrowstyle="->", color=SECONDARY_ORANGE, lw=1.3),
    )

    ax.set_title("Migration Time Trend — WingtipToys (Runs 1–90)")
    ax.set_xlabel("WingtipToys benchmark run")
    ax.set_ylabel("Elapsed time")
    ax.set_xticks(runs)
    ax.yaxis.set_major_formatter(FuncFormatter(format_seconds))
    ax.set_ylim(0, 1450)
    finalize_axes(ax)
    ax.legend(loc="upper right", ncol=3)
    save_figure(fig, "migration-time.svg")


def generate_acceptance_tests_chart() -> None:
    runs =   np.array([10, 40, 70, 76, 77, 78, 79, 80, 81, 88, 89, 90])
    passed = np.array([20, 25, 25, 23, 25, 25, 25, 25, 25, 26, 26, 26])
    total =  np.array([25, 25, 25, 25, 25, 25, 25, 25, 25, 26, 26, 26])
    failed = total - passed

    fig, ax = plt.subplots(figsize=(13, 5.8))

    ax.bar(runs, passed, color=SUCCESS_GREEN, width=2.4, label="Passed")
    ax.bar(runs, failed, bottom=passed, color=FAILURE_RED, width=2.4, label="Failed")

    for run, pass_count, fail_count, tot in zip(runs, passed, failed, total):
        ax.text(run, pass_count - 0.8 if pass_count > 2 else pass_count + 0.2,
                f"{pass_count}/{tot}",
                ha="center", va="top" if pass_count > 2 else "bottom",
                color="white" if pass_count > 2 else "#0f172a",
                fontsize=9, fontweight="bold")
        if fail_count > 0:
            ax.text(run, pass_count + fail_count - 0.45, f"{int(fail_count)} fail",
                    ha="center", va="top", color="white", fontsize=8)

    ax.annotate(
        "Run 76 dip while regression work was in flight",
        xy=(76, 23),
        xytext=(55, 18),
        fontsize=10,
        color="#991b1b",
        arrowprops=dict(arrowstyle="->", color=FAILURE_RED, lw=1.3),
        bbox=dict(boxstyle="round,pad=0.35", facecolor="#fef2f2", edgecolor="#fecaca"),
    )
    ax.annotate(
        "Run 88: 26th test added\n(add-to-cart flow)",
        xy=(88, 26),
        xytext=(74, 24.5),
        fontsize=9,
        color="#065f46",
        arrowprops=dict(arrowstyle="->", color=SUCCESS_GREEN, lw=1.3),
        bbox=dict(boxstyle="round,pad=0.35", facecolor="#f0fdf4", edgecolor="#86efac"),
    )

    ax.set_title("Acceptance Test Results — WingtipToys (Runs 10–90)")
    ax.set_xlabel("WingtipToys benchmark run")
    ax.set_ylabel("Acceptance tests")
    ax.set_xticks(runs)
    ax.set_ylim(0, 29)
    finalize_axes(ax)
    ax.legend(loc="upper left")
    save_figure(fig, "acceptance-tests.svg")


def generate_runtime_performance_chart() -> None:
    pages = ["WT Home", "WT ProductList", "WT About", "CU Home", "CU Students", "CU About"]
    web_forms = np.array([6.4, 8.3, 4.9, 2.2, 6.5, 3.6])
    blazor = np.array([2.4, 3.6, 3.1, 1.6, 6.3, 2.8])

    x = np.arange(len(pages))
    width = 0.36

    fig, ax = plt.subplots(figsize=(11.5, 6.2))
    wf_bars = ax.bar(x - width / 2, web_forms, width, color=LIGHT_ORANGE, edgecolor=SECONDARY_ORANGE, linewidth=1.2, label="Web Forms")
    blazor_bars = ax.bar(x + width / 2, blazor, width, color=LIGHT_BLUE, edgecolor=PRIMARY_BLUE, linewidth=1.2, label="Blazor")

    for bar in list(wf_bars) + list(blazor_bars):
        ax.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 0.12, f"{bar.get_height():.1f}", ha="center", va="bottom", fontsize=9, color="#0f172a")

    speedups = web_forms / blazor
    for idx, speedup in enumerate(speedups):
        ax.text(x[idx], max(web_forms[idx], blazor[idx]) + 0.7, f"{speedup:.2f}× faster", ha="center", va="bottom", fontsize=9, color=SUCCESS_GREEN, fontweight="bold")

    ax.set_title("Runtime Performance Comparison")
    ax.set_xlabel("Benchmark page")
    ax.set_ylabel("Average response time (ms)")
    ax.set_xticks(x)
    ax.set_xticklabels(pages)
    ax.set_ylim(0, 10.2)
    finalize_axes(ax)
    ax.legend(loc="upper right")
    save_figure(fig, "runtime-performance.svg")


def main() -> None:
    configure_style()
    generate_error_reduction_chart()
    generate_migration_time_chart()
    generate_acceptance_tests_chart()
    generate_runtime_performance_chart()
    generate_dual_benchmark_chart()


def generate_dual_benchmark_chart() -> None:
    """Side-by-side comparison: WingtipToys vs ContosoUniversity key metrics."""
    fig, axes = plt.subplots(1, 2, figsize=(15, 6.2))

    # --- Left panel: Migration time by phase ---
    ax1 = axes[0]
    apps = ["WingtipToys\n(Run 90)", "ContosoUniversity\n(Run 30)"]
    l1_seconds    = np.array([22,    7])   # confirmed L1 durations
    l2_seconds    = np.array([61,  180])   # WT: 1:01; CU: ~3 min build repair
    other_seconds = np.array([68,  360])   # WT: startup 37s + tests 31s; CU: startup+tests ~6 min

    x = np.arange(len(apps))
    width = 0.5

    b1 = ax1.bar(x, l1_seconds,    width, label="L1 automated",  color=SUCCESS_GREEN,    edgecolor="white")
    b2 = ax1.bar(x, l2_seconds,    width, bottom=l1_seconds,                    label="L2 build repair", color=SECONDARY_ORANGE, edgecolor="white")
    b3 = ax1.bar(x, other_seconds, width, bottom=l1_seconds + l2_seconds,       label="Startup + tests", color=LIGHT_BLUE,        edgecolor=PRIMARY_BLUE)

    totals = l1_seconds + l2_seconds + other_seconds
    labels = ["2:31\n(6:24 w/ report)", "~9:07\n(21.8 min w/ L2 repair)"]
    for idx, (tot, lbl) in enumerate(zip(totals, labels)):
        ax1.text(idx, tot + 18, lbl, ha="center", va="bottom", fontsize=9, fontweight="bold", color="#0f172a")

    ax1.set_title("Automated Pipeline by Phase")
    ax1.set_xlabel("Benchmark Application")
    ax1.set_ylabel("Time (automated phases only)")
    ax1.set_xticks(x)
    ax1.set_xticklabels(apps)
    ax1.yaxis.set_major_formatter(FuncFormatter(format_seconds))
    ax1.set_ylim(0, 720)
    ax1.legend(loc="upper right")
    finalize_axes(ax1)

    # --- Right panel: Acceptance tests — recent runs for both apps ---
    ax2 = axes[1]

    wt_runs   = np.array([81, 88, 89, 90])
    wt_passed = np.array([25, 26, 26, 26])
    wt_total  = np.array([25, 26, 26, 26])

    cu_runs   = np.array([27, 28, 29, 30])
    cu_passed = np.array([40, 37, 40, 40])
    cu_total  = np.array([40, 40, 40, 40])

    wt_x = np.arange(len(wt_runs))
    cu_x = np.arange(len(cu_runs)) + len(wt_runs) + 1.5

    ax2.bar(wt_x, wt_passed, color=SUCCESS_GREEN, width=0.7, label="WingtipToys")
    ax2.bar(wt_x, wt_total - wt_passed, bottom=wt_passed, color=FAILURE_RED, width=0.7)
    ax2.bar(cu_x, cu_passed, color=PRIMARY_BLUE, width=0.7, label="ContosoUniversity")
    ax2.bar(cu_x, cu_total - cu_passed, bottom=cu_passed, color=FAILURE_RED, width=0.7)

    for xi, (run, p, t) in enumerate(zip(wt_runs, wt_passed, wt_total)):
        ax2.text(wt_x[xi], p - 1.2, f"{p}/{t}", ha="center", va="top", color="white", fontsize=9, fontweight="bold")
    for xi, (run, p, t) in enumerate(zip(cu_runs, cu_passed, cu_total)):
        ax2.text(cu_x[xi], p - 2.0, f"{p}/{t}", ha="center", va="top", color="white", fontsize=9, fontweight="bold")

    # Separator line
    ax2.axvline(x=len(wt_runs) + 0.75, color=NEUTRAL_GRAY, linestyle=":", linewidth=1.2, alpha=0.6)
    ax2.text(len(wt_runs) + 0.75, 42.5, "WT → CU", ha="center", va="bottom", fontsize=8, color=NEUTRAL_GRAY)

    xt_labels = [f"WT\nRun {r}" for r in wt_runs] + [""] + [f"CU\nRun {r}" for r in cu_runs]
    ax2.set_xticks(list(wt_x) + [len(wt_runs) + 0.75] + list(cu_x))
    ax2.set_xticklabels(xt_labels, fontsize=8)
    ax2.set_title("Acceptance Tests — Both Benchmarks (Recent Runs)")
    ax2.set_xlabel("Benchmark run")
    ax2.set_ylabel("Acceptance tests")
    ax2.set_ylim(0, 46)
    ax2.legend(loc="lower right")
    finalize_axes(ax2)

    save_figure(fig, "dual-benchmark.svg")


if __name__ == "__main__":
    main()
