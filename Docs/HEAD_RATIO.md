# Measured head proportions - UPDATE3

Target: 43% of crown-to-sole height, tolerance 3 percentage points. Crown uses authored head-top anchors; chin/neck boundary uses the largest upper warm-skin component plus a 6px outline allowance. Excludes items raised above the crown, trailing hair and visual effects. This is an explicit reproducible landmark measurement, not an unlabelled claim of anatomical segmentation. Original source frames/landmarks are preserved in ArtSource/ProportionOriginals.

Continuous whole-image mesh correction keeps the canvas 896x512, feet pivot (448,512), ground line and baked-in weapons. No runtime parts rig. Portraits are cropped from the corrected ready pose; runtime previews use the same corrected idle frames.

| Character | Ready before | Ready after | All 12 source poses after |
|---|---:|---:|---:|
| AlingPamalo | 40.5% | 42.9% | 41.2%–43.1% |
| KuyaTsinelas | 40.9% | 40.9% | 40.5%–43.8% |
| AteBatoBatoPik | 42.0% | 42.0% | 40.6%–44.9% |
| KapBatuta | 44.0% | 44.0% | 42.7%–44.9% |
| ManongLatalata | 39.2% | 43.1% | 41.2%–43.1% |
| TitoSilya | 48.8% | 42.9% | 42.9%–43.1% |
| AlingBotengSuka | 55.8% | 43.0% | 42.9%–43.1% |
| NanayTingting | 56.5% | 42.9% | 42.9%–43.1% |

All 272 runtime frames are bound by SHA-256 to the measured source-pose ratio in HeadRatios.json. Run `python Tools/test_head_ratios.py` after art changes; rerun this importer after reimporting source sheets.
