# Active typography

One OFL font family: Tiny5, stored as the static StreetPixel TMP asset. Point-filtered 1024-square atlas, padding 6. No default Arial/LegacyRuntime font is used by active UI.

| Role | Reference size at 1280 x 720 | Treatment |
| --- | --- | --- |
| Logo / title | 80?82 | Warm cream, subtle dark outline |
| Heading | 48 | Cream, centered |
| Character / result name | 25?32 | Player accent or warm gold |
| Body | 23?25 | Cream on dark panel |
| Hint | 18?22 | Warm gold |
| Reaction bubble | 23 | Dark ink on cream |

The shared material defines a small dark outline; UI labels use a dark offset shadow where supported. All labels are TMP objects. Required ASCII punctuation/digits and ?/? are explicitly packed and checked by automated tests.
