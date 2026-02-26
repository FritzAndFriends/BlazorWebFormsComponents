# HTML Audit Comparison Report

Generated: 2026-02-26T14:29:23.916Z

## Summary
- Controls compared: 132
- Exact matches: 1
- Divergences found: 131

## Results by Control

### AdRotator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| AdRotator | ⚠️ Divergent | Tag structure differs |

#### AdRotator Diff
```diff
- <a href="https://bing.com" id="AdRotator1" target="_top"><img alt="Visit Bing" src="/Content/Images/banner.png" /></a>
+ <a href="http://www.microsoft.com" target="_top"><img alt="CSharp" height="343" src="/img/CSharp.png" width="397" /></a>
```

### BulletedList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| BulletedList-1 | ⚠️ Divergent | Tag structure differs |
| BulletedList-2 | ⚠️ Divergent | Tag structure differs |
| BulletedList-3 | ⚠️ Divergent | Tag structure differs |

#### BulletedList-1 Diff
```diff
- <ul id="blDisc" style="list-style-type:disc;">
+ <ul><li>First item</li><li>Second item</li><li>Third item</li></ul>
- <li>Apple</li><li>Banana</li><li>Cherry</li><li>Date</li>
- </ul>
```

#### BulletedList-2 Diff
```diff
- <ol id="blNumbered" style="list-style-type:decimal;">
+ <ul style="list-style-type:disc;"><li>Item One</li><li>Item Two</li><li>Item Three</li><li>Item Four</li></ul>
- <li>First</li><li>Second</li><li>Third</li>
- </ol>
```

#### BulletedList-3 Diff
```diff
- <ul id="blSquare" style="list-style-type:square;">
+ <ul style="list-style-type:circle;"><li>Item One</li><li>Item Two</li><li>Item Three</li><li>Item Four</li></ul>
- <li><a href="https://example.com">Example Site</a></li><li><a href="https://example.org">Example Org</a></li>
- </ul>
```

### Button
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Button-1 | ⚠️ Divergent | Tag structure differs |
| Button-2 | ❌ Missing in source B | File only exists in first directory |
| Button-3 | ❌ Missing in source B | File only exists in first directory |
| Button-4 | ❌ Missing in source B | File only exists in first directory |
| Button-5 | ❌ Missing in source B | File only exists in first directory |

#### Button-1 Diff
```diff
- <input id="styleButton" style="color:#ffffff; background-color:#0000ff;" type="submit" value="Blue Button" />
+ <input accesskey="b" style="" title="Click to submit" type="submit" value="Click me!" />
```

### Calendar
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Calendar-1 | ⚠️ Divergent | Tag structure differs |
| Calendar-2 | ⚠️ Divergent | Tag structure differs |
| Calendar-3 | ⚠️ Divergent | Tag structure differs |
| Calendar-4 | ⚠️ Divergent | Tag structure differs |
| Calendar-5 | ⚠️ Divergent | Tag structure differs |
| Calendar-6 | ⚠️ Divergent | Tag structure differs |
| Calendar-7 | ⚠️ Divergent | Tag structure differs |

#### Calendar-1 Diff
```diff
- <table id="Calendar1" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-2 Diff
```diff
- <table id="CalendarDay" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><span>1</span></td><td align="center" style="width:14%;"><span>2</span></td><td align="center" style="width:14%;"><span>3</span></td><td align="center" style="width:14%;"><span>4</span></td><td align="center" style="width:14%;"><span>5</span></td><td align="center" style="width:14%;"><span>6</span></td><td align="center" style="width:14%;"><span>7</span></td></tr><tr><td align="center" style="width:14%;"><span>8</span></td><td align="center" style="width:14%;"><span>9</span></td><td align="center" style="width:14%;"><span>10</span></td><td align="center" style="width:14%;"><span>11</span></td><td align="center" style="width:14%;"><span>12</span></td><td align="center" style="width:14%;"><span>13</span></td><td align="center" style="width:14%;"><span>14</span></td></tr><tr><td align="center" style="width:14%;"><span>15</span></td><td align="center" style="width:14%;"><span>16</span></td><td align="center" style="width:14%;"><span>17</span></td><td align="center" style="width:14%;"><span>18</span></td><td align="center" style="width:14%;"><span>19</span></td><td align="center" style="width:14%;"><span>20</span></td><td align="center" style="width:14%;"><span>21</span></td></tr><tr><td align="center" style="width:14%;"><span>22</span></td><td align="center" style="width:14%;"><span>23</span></td><td align="center" style="width:14%;"><span>24</span></td><td align="center" style="width:14%;"><span>25</span></td><td align="center" style="width:14%;"><span>26</span></td><td align="center" style="width:14%;"><span>27</span></td><td align="center" style="width:14%;"><span>28</span></td></tr><tr><td align="center" style="width:14%;"><span>1</span></td><td align="center" style="width:14%;"><span>2</span></td><td align="center" style="width:14%;"><span>3</span></td><td align="center" style="width:14%;"><span>4</span></td><td align="center" style="width:14%;"><span>5</span></td><td align="center" style="width:14%;"><span>6</span></td><td align="center" style="width:14%;"><span>7</span></td></tr><tr><td align="center" style="width:14%;"><span>8</span></td><td align="center" style="width:14%;"><span>9</span></td><td align="center" style="width:14%;"><span>10</span></td><td align="center" style="width:14%;"><span>11</span></td><td align="center" style="width:14%;"><span>12</span></td><td align="center" style="width:14%;"><span>13</span></td><td align="center" style="width:14%;"><span>14</span></td></tr></tbody></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-3 Diff
```diff
- <table id="CalendarWeek" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="8" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><td align="center"></td><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-4 Diff
```diff
- <table id="CalendarMonth" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td></td><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th></th><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="8" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><td align="center"><a style="color:#000000;" title="Select the whole month">&gt;&gt;</a></td><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a style="color:#000000;" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:12%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-5 Diff
```diff
- <table id="CalendarStyled" style="color:#000000; background-color:#ffffff; border-color:#999999; border-width:1px; border-style:solid; font-family:verdana; font-size:8pt; width:220px; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td><a style="cursor:pointer;">&gt;&gt;</a></td><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th></th><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="7" style="background-color:#ccccff;"><table style="color:#333399; font-family:verdana; font-size:13pt; font-weight:bold; width:100%; border-collapse:collapse;">
- <tbody><tr><td style="color:#333399; font-size:8pt; width:15%;" valign="bottom"><a style="color:#333399;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="color:#333399; font-size:8pt; width:15%;" valign="bottom"><a style="color:#333399;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Su</th><th abbr="Monday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Mo</th><th abbr="Tuesday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Tu</th><th abbr="Wednesday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">We</th><th abbr="Thursday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Th</th><th abbr="Friday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Fr</th><th abbr="Saturday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Sa</th></tr><tr><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a style="color:#999999;" title="January 25">25</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="January 26">26</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="January 27">27</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="January 28">28</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="January 29">29</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="January 30">30</a></td><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a style="color:#999999;" title="January 31">31</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="color:#000000; background-color:#ccccff; width:14%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a style="color:#999999;" title="March 1">1</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="March 2">2</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="March 3">3</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="March 4">4</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="March 5">5</a></td><td align="center" style="color:#999999; width:14%;"><a style="color:#999999;" title="March 6">6</a></td><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a style="color:#999999;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-6 Diff
```diff
- <table id="CalendarNav" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><td colspan="7"><table style="width:100%; border-collapse:collapse;"><tbody><tr><td style="width:15%;"><a style="cursor:pointer;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="cursor:pointer;" title="Go to the next month">&gt;</a></td></tr></tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">« Prev</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">Next »</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-7 Diff
```diff
- <table id="CalendarEvents" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table style="border-width:1px; border-style:solid; border-collapse:collapse;"><tbody><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 8">8</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 9">9</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 10">10</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 11">11</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 12">12</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 13">13</a></td><td align="center" style="width:14%;"><a style="cursor:pointer;" title="March 14">14</a></td></tr></tbody></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

### ChangePassword
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ChangePassword-1 | ❌ Missing in source B | File only exists in first directory |
| ChangePassword-2 | ❌ Missing in source B | File only exists in first directory |

### CheckBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CheckBox-1 | ⚠️ Divergent | Tag structure differs |
| CheckBox-2 | ⚠️ Divergent | Tag structure differs |
| CheckBox-3 | ⚠️ Divergent | Tag structure differs |

#### CheckBox-1 Diff
```diff
- <input id="chkTerms" style="" type="checkbox" /><label for="MainContent_chkTerms">Accept Terms</label>
+ <input id="20c6503f0cca4e2ab1ccfd84915c979a" style="" type="checkbox" />
+ <label for="20c6503f0cca4e2ab1ccfd84915c979a">I agree to terms</label>
```

#### CheckBox-2 Diff
```diff
- <input checked="checked" id="chkSubscribe" style="" type="checkbox" /><label for="MainContent_chkSubscribe">Subscribe</label>
+ <input id="399517551d3b41299b84a19e72333037" style="" type="checkbox" />
+ <label for="399517551d3b41299b84a19e72333037">Already checked</label>
```

#### CheckBox-3 Diff
```diff
- <input id="chkFeature" style="" type="checkbox" /><label for="MainContent_chkFeature">Enable Feature</label>
+ <input id="126e626b352b4405b279923ae1b94405" style="" type="checkbox" />
+ <label for="126e626b352b4405b279923ae1b94405">Label on right (default)</label>
```

### CheckBoxList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CheckBoxList-1 | ⚠️ Divergent | Tag structure differs |
| CheckBoxList-2 | ⚠️ Divergent | Tag structure differs |

#### CheckBoxList-1 Diff
```diff
- <table id="CheckBoxList1">
+ <table><tr><td><input id="8a9ff70d_0" name="8a9ff70d$0" style="" type="checkbox" value="red" /><label for="8a9ff70d_0">Red</label></td></tr><tr><td><input id="8a9ff70d_1" name="8a9ff70d$1" style="" type="checkbox" value="green" /><label for="8a9ff70d_1">Green</label></td></tr><tr><td><input id="8a9ff70d_2" name="8a9ff70d$2" style="" type="checkbox" value="blue" /><label for="8a9ff70d_2">Blue</label></td></tr><tr><td><input id="8a9ff70d_3" name="8a9ff70d$3" style="" type="checkbox" value="yellow" /><label for="8a9ff70d_3">Yellow</label></td></tr></table>
- <tbody><tr>
- <td><input id="CheckBoxList1_0" style="" type="checkbox" value="Reading" /><label for="MainContent_CheckBoxList1_0">Reading</label></td>
- </tr><tr>
- <td><input id="CheckBoxList1_1" style="" type="checkbox" value="Sports" /><label for="MainContent_CheckBoxList1_1">Sports</label></td>
- </tr><tr>
- <td><input checked="checked" id="CheckBoxList1_2" style="" type="checkbox" value="Music" /><label for="MainContent_CheckBoxList1_2">Music</label></td>
- </tr><tr>
- <td><input id="CheckBoxList1_3" style="" type="checkbox" value="Travel" /><label for="MainContent_CheckBoxList1_3">Travel</label></td>
- </tr>
- </tbody></table>
```

#### CheckBoxList-2 Diff
```diff
- <table id="CheckBoxList2">
+ <span><input id="1ad35129_0" name="1ad35129$0" style="" type="checkbox" value="S" /><label for="1ad35129_0">Small</label><input id="1ad35129_1" name="1ad35129$1" style="" type="checkbox" value="M" /><label for="1ad35129_1">Medium</label><input id="1ad35129_2" name="1ad35129$2" style="" type="checkbox" value="L" /><label for="1ad35129_2">Large</label><input id="1ad35129_3" name="1ad35129$3" style="" type="checkbox" value="XL" /><label for="1ad35129_3">X-Large</label></span>
- <tbody><tr>
- <td><input id="CheckBoxList2_0" style="" type="checkbox" value="Small" /><label for="MainContent_CheckBoxList2_0">Small</label></td><td><input id="CheckBoxList2_1" style="" type="checkbox" value="Medium" /><label for="MainContent_CheckBoxList2_1">Medium</label></td>
- </tr><tr>
- <td><input id="CheckBoxList2_2" style="" type="checkbox" value="Large" /><label for="MainContent_CheckBoxList2_2">Large</label></td><td><input id="CheckBoxList2_3" style="" type="checkbox" value="XLarge" /><label for="MainContent_CheckBoxList2_3">XLarge</label></td>
- </tr>
- </tbody></table>
```

### CompareValidator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CompareValidator-1 | ❌ Missing in source B | File only exists in first directory |
| CompareValidator-2 | ❌ Missing in source B | File only exists in first directory |
| CompareValidator-3 | ❌ Missing in source B | File only exists in first directory |
| CompareValidator-Submit | ❌ Missing in source B | File only exists in first directory |

### CreateUserWizard
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CreateUserWizard-1 | ❌ Missing in source B | File only exists in first directory |
| CreateUserWizard-2 | ❌ Missing in source B | File only exists in first directory |

### CustomValidator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CustomValidator-1 | ❌ Missing in source B | File only exists in first directory |
| CustomValidator-2 | ❌ Missing in source B | File only exists in first directory |
| CustomValidator-Submit | ❌ Missing in source B | File only exists in first directory |

### DataList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| DataList | ⚠️ Divergent | 106 line differences |

#### DataList Diff
```diff
- <table id="simpleDataList" itemtype="SharedSampleObjects.Models.Widget" tabindex="1" title="This is my tooltip">
+ <table><tbody><tr><td>Simple Widgets</td></tr><tr><td style="background-color:#f5deb3;">First Widget - $7.99</td></tr><tr><td style="background-color:#f5deb3;">Second Widget - $13.99</td></tr><tr><td style="background-color:#f5deb3;">Third Widget - $100.99</td></tr><tr><td style="background-color:#f5deb3;">Fourth Widget - $10.99</td></tr><tr><td style="background-color:#f5deb3;">Fifth Widget - $5.99</td></tr><tr><td style="background-color:#f5deb3;">Sixth Widget - $6.99</td></tr><tr><td style="background-color:#f5deb3;">Seventh Widget - $12.99</td></tr><tr><td style="background-color:#f5deb3;">Eighth Widget - $8.99</td></tr><tr><td style="background-color:#f5deb3;">Ninth Widget - $2.99</td></tr><tr><td style="background-color:#f5deb3;">Tenth Widget - $3.99</td></tr><tr><td style="background-color:#f5deb3;">Eleventh Widget - $16.99</td></tr><tr><td style="background-color:#f5deb3;">Fritz's Widget - $52.70</td></tr><tr><td>End of Line</td></tr></tbody></table>
- <caption align="Top">
- This is my caption
- </caption><tbody><tr>
- <th class="myClass" scope="col" style="font-family:arial #000000; font-size:x-large; font-weight:bold; font-style:italic; text-decoration:underline overline line-through;">
- My Widget List
- </th>
- </tr><tr>
- <td style="background-color:#ffff00; white-space:nowrap;">
- First Widget
- <br/>
- $7.99
- </td>
- </tr><tr>
- <td style="color:#ffefd5; background-color:#000000;">Hi!  I'm a separator!  I keep things apart</td>
- </tr><tr>
- <td style="background-color:#f5deb3; white-space:nowrap;">
- Second Widget
- <br/>
- $13.99
- </td>
- </tr><tr>
- <td style="color:#ffefd5; background-color:#000000;">Hi!  I'm a separator!  I keep things apart</td>
- </tr><tr>
- <td style="background-color:#ffff00; white-space:nowrap;">
- Third Widget
- <br/>
- $100.99
- </td>
- </tr><tr>
- <td style="color:#ffefd5; background-color:#000000;">Hi!  I'm a separator!  I keep things apart</td>
- </tr><tr>
- <td style="background-color:#f5deb3; white-space:nowrap;">
- Fourth Widget
- <br/>
- $10.99
- </td>
- </tr><tr>
- <td style="color:#ffefd5; background-color:#000000;">Hi!  I'm a separator!  I keep things apart</td>
... (truncated)
```

### DataPager
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| DataPager | ❌ Missing in source B | File only exists in first directory |

### DetailsView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| DetailsView-1 | ❌ Missing in source A | File only exists in second directory |
| DetailsView-2 | ❌ Missing in source A | File only exists in second directory |
| DetailsView-3 | ❌ Missing in source A | File only exists in second directory |
| DetailsView-4 | ❌ Missing in source A | File only exists in second directory |
| DetailsView | ❌ Missing in source B | File only exists in first directory |

### DropDownList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| DropDownList-1 | ⚠️ Divergent | Tag structure differs |
| DropDownList-2 | ⚠️ Divergent | Tag structure differs |
| DropDownList-3 | ⚠️ Divergent | Tag structure differs |
| DropDownList-4 | ⚠️ Divergent | Tag structure differs |
| DropDownList-5 | ⚠️ Divergent | Tag structure differs |
| DropDownList-6 | ⚠️ Divergent | Tag structure differs |

#### DropDownList-1 Diff
```diff
- <select id="ddlStatic">
+ <select><option selected="" value="">Select...</option><option value="1">Option One</option><option value="2">Option Two</option><option value="3">Option Three</option></select>
- <option value="">Select...</option>
- <option value="1">Option One</option>
- <option value="2">Option Two</option>
- <option value="3">Option Three</option>
- </select>
```

#### DropDownList-2 Diff
```diff
- <select id="ddlSelected">
+ <select><option value="apple">Apple</option><option selected="" value="banana">Banana</option><option value="cherry">Cherry</option></select>
- <option value="apple">Apple</option>
- <option selected="selected" value="banana">Banana</option>
- <option value="cherry">Cherry</option>
- </select>
```

#### DropDownList-3 Diff
```diff
- <select id="ddlDataBound">
+ <select><option value="1">Widget</option><option value="2">Gadget</option><option value="3">Gizmo</option><option value="4">Doohickey</option></select>
- <option value="1">First Item</option>
- <option value="2">Second Item</option>
- <option value="3">Third Item</option>
- </select>
```

#### DropDownList-4 Diff
```diff
- <select class="aspNetDisabled" disabled="disabled" id="ddlDisabled">
+ <select><option value="1">9.99</option><option value="2">24.50</option><option value="3">149.95</option><option value="4">3.00</option></select>
- <option selected="selected" value="1">Cannot change</option>
- </select>
```

#### DropDownList-5 Diff
```diff
- <select class="form-select" id="ddlStyled">
+ <select><option value="1">Item: Widget</option><option value="2">Item: Gadget</option><option value="3">Item: Gizmo</option><option value="4">Item: Doohickey</option></select>
- <option value="1">Styled</option>
- </select>
```

#### DropDownList-6 Diff
```diff
- <select id="ddlColors" style="color:#000080; background-color:#ffffe0; width:200px;">
+ <select disabled=""><option value="">Select...</option><option value="1">Option One</option><option selected="" value="2">Option Two</option><option value="3">Option Three</option></select>
- <option value="1">Colored dropdown</option>
- </select>
```

### FileUpload
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| FileUpload | ⚠️ Divergent | Tag structure differs |

#### FileUpload Diff
```diff
- <input id="FileUpload1" style="" type="file" />
+ <input f16-8aa4-bed3eff8548b="" style="" type="file" />
- <input id="btnUpload" style="" type="submit" value="Upload" />
```

### FormView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| FormView | ❌ Missing in source B | File only exists in first directory |

### GridView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| GridView | ⚠️ Divergent | Tag structure differs |

#### GridView Diff
```diff
- <div>
+ <table class="table table-striped" style="border-collapse:collapse;"><thead><tr><th>ID</th><th>CompanyName</th><th>FirstName</th><th>LastName</th><th>&amp;nbsp;</th><th>&amp;nbsp;</th></tr></thead><tbody><tr><td>1</td><td>Virus</td><td>John</td><td>Smith</td><td><button type="button">Click Me! John</button></td><td><input style="" type="submit" /></td></tr><tr><td>2</td><td>Boring</td><td>Jose</td><td>Rodriguez</td><td><button type="button">Click Me! Jose</button></td><td><input style="" type="submit" /></td></tr><tr><td>3</td><td>Fun Machines</td><td>Jason</td><td>Ramirez</td><td><button type="button">Click Me! Jason</button></td><td><input style="" type="submit" /></td></tr></tbody></table>
- <table id="CustomersGridView" style="border-collapse:collapse;">
- <tbody><tr>
- <th scope="col">CustomerID</th><th scope="col">CompanyName</th><th scope="col">FirstName</th><th scope="col">LastName</th><th scope="col">&nbsp;</th><th scope="col">&nbsp;</th>
- </tr><tr>
- <td>1</td><td>Virus</td><td>John</td><td>Smith</td><td>
- <button type="button">Click Me! John</button>
- </td><td><a href="https://www.bing.com/search?q=Virus John Smith">Search for Virus</a></td>
- </tr><tr>
- <td>2</td><td>Boring</td><td>Jose</td><td>Rodriguez</td><td>
- <button type="button">Click Me! Jose</button>
- </td><td><a href="https://www.bing.com/search?q=Boring Jose Rodriguez">Search for Boring</a></td>
- </tr><tr>
- <td>3</td><td>Fun Machines</td><td>Jason</td><td>Ramirez</td><td>
- <button type="button">Click Me! Jason</button>
- </td><td><a href="https://www.bing.com/search?q=Fun Machines Jason Ramirez">Search for Fun Machines</a></td>
- </tr>
- </tbody></table>
- </div>
```

### HiddenField
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| HiddenField | ⚠️ Divergent | Tag structure differs |

#### HiddenField Diff
```diff
- <input id="HiddenField1" style="" type="hidden" value="secret-value-123" />
+ <input id="myHiddenField" style="" type="hidden" value="initial-secret-value" />
```

### Hyperlink
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| HyperLink-1 | ⚠️ Divergent | Tag structure differs |
| HyperLink-2 | ⚠️ Divergent | Tag structure differs |
| HyperLink-3 | ⚠️ Divergent | Tag structure differs |
| HyperLink-4 | ⚠️ Divergent | Tag structure differs |

#### HyperLink-1 Diff
```diff
- <a href="https://bing.com" id="styleLink" style="color:#ffffff; background-color:#0000ff;">Blue Button</a>
+ <a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a href="https://bing.com" id="HyperLink1" title="Navigate to Bing!">Blue Button</a>
+ <a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a href="https://bing.com" id="HyperLink3">Blue Button</a>
+ <a href="https://www.github.com" style="background-color:#696969; color:#ffffff;" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

### HyperLink
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| HyperLink-1 | ⚠️ Divergent | Tag structure differs |
| HyperLink-2 | ⚠️ Divergent | Tag structure differs |
| HyperLink-3 | ⚠️ Divergent | Tag structure differs |
| HyperLink-4 | ⚠️ Divergent | Tag structure differs |

#### HyperLink-1 Diff
```diff
- <a href="https://bing.com" id="styleLink" style="color:#ffffff; background-color:#0000ff;">Blue Button</a>
+ <a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a href="https://bing.com" id="HyperLink1" title="Navigate to Bing!">Blue Button</a>
+ <a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a href="https://bing.com" id="HyperLink3">Blue Button</a>
+ <a href="https://www.github.com" style="background-color:#696969; color:#ffffff;" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

### Image
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Image-1 | ⚠️ Divergent | Tag structure differs |
| Image-2 | ⚠️ Divergent | Tag structure differs |

#### Image-1 Diff
```diff
- <img alt="Banner image" id="imgBasic" src="../../Content/Images/banner.png" />
+ <img alt="Sample placeholder image" src="/img/placeholder-150x100.svg" />
```

#### Image-2 Diff
```diff
- <img alt="Sized image" id="imgSized" src="../../Content/Images/banner.png" style="height:100px; width:200px;" />
+ <img alt="Image with tooltip" src="/img/placeholder-150x100.svg" title="This is a tooltip displayed on hover" />
```

### ImageButton
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ImageButton-1 | ⚠️ Divergent | Tag structure differs |
| ImageButton-2 | ⚠️ Divergent | Tag structure differs |

#### ImageButton-1 Diff
```diff
- <input alt="Submit" id="ImageButton1" src="../../Content/Images/banner.png" style="" type="image" />
+ <input alt="Submit" longdesc="" src="/img/placeholder-150x100.svg" style="" type="image" />
```

#### ImageButton-2 Diff
```diff
- <input alt="Click here" class="img-button" id="ImageButton2" src="../../Content/Images/banner.png" style="" type="image" />
+ <input alt="Styled submit button" class="img-button-styled" longdesc="" src="/img/placeholder-80x80.svg" style="" title="Click this image button to submit" type="image" />
```

### ImageMap
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ImageMap | ⚠️ Divergent | Tag structure differs |

#### ImageMap Diff
```diff
- <img alt="Navigate" id="ImageMap1" src="../../Content/Images/banner.png" usemap="#ImageMapMainContent_ImageMap1" /><map id="ImageMapMainContent_ImageMap1" name="ImageMapMainContent_ImageMap1">
+ <img alt="Navigation demo image" src="/img/placeholder-400x200.svg" usemap="#ImageMap_63b30349dd5343b89b038301c5723eda" /><map name="ImageMap_63b30349dd5343b89b038301c5723eda"><area alt="Go to Button samples" coords="0,0,130,200" href="/ControlSamples/Button" shape="rect" /><area alt="Go to CheckBox samples" coords="200,100,60" href="/ControlSamples/CheckBox" shape="circle" /><area alt="Go to Image samples" coords="270,0,400,200" href="/ControlSamples/Image" shape="rect" /></map>
- <area alt="Go to Bing" coords="0,0,100,50" href="https://bing.com" shape="rect" title="Go to Bing" /><area alt="Go to GitHub" coords="100,0,200,50" href="https://github.com" shape="rect" title="Go to GitHub" />
- </map>
```

### Label
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Label-1 | ⚠️ Divergent | Tag structure differs |
| Label-2 | ⚠️ Divergent | Tag structure differs |
| Label-3 | ⚠️ Divergent | Tag structure differs |

#### Label-1 Diff
```diff
- <span id="lblBasic">Hello World</span>
+ <span>Hello, World!</span>
```

#### Label-2 Diff
```diff
- <span class="text-primary" id="lblStyled" style="color:#0000ff; font-weight:bold;">Styled Label</span>
+ <span class="text-danger fw-bold" style="color:#ff0000;">Important notice</span>
```

#### Label-3 Diff
```diff
- <span id="lblHtml"><em>Emphasized</em></span>
+ <label class="form-label" for="emailInput">Email Address:</label>
```

### LinkButton
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| LinkButton-1 | ⚠️ Divergent | Tag structure differs |
| LinkButton-2 | ⚠️ Divergent | Tag structure differs |
| LinkButton-3 | ⚠️ Divergent | Tag structure differs |

#### LinkButton-1 Diff
```diff
- <a class="btn btn-primary" id="LinkButton1">Click Me</a>
+ <a href="javascript:void(0)">LinkButton1 with Command</a>
```

#### LinkButton-2 Diff
```diff
- <a id="LinkButton2">Submit Form</a>
+ <a href="javascript:void(0)">LinkButton2 with Command</a>
```

#### LinkButton-3 Diff
```diff
- <a class="aspNetDisabled" id="LinkButton3">Disabled Link</a>
+ <a href="ControlSamples/LinkButton/#">Click me!!</a>
```

### ListBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ListBox-1 | ⚠️ Divergent | Tag structure differs |
| ListBox-2 | ⚠️ Divergent | Tag structure differs |

#### ListBox-1 Diff
```diff
- <select id="ListBox1" size="5">
+ <select size="4"><option value="red">Red</option><option value="green">Green</option><option value="blue">Blue</option><option value="yellow">Yellow</option><option value="purple">Purple</option></select>
- <option value="Red">Red</option>
- <option value="Orange">Orange</option>
- <option value="Yellow">Yellow</option>
- <option value="Green">Green</option>
- <option value="Blue">Blue</option>
- <option value="Purple">Purple</option>
- </select>
```

#### ListBox-2 Diff
```diff
- <select id="ListBox2" multiple="multiple" size="4">
+ <select multiple="" size="4"><option value="S">Small</option><option value="M">Medium</option><option value="L">Large</option><option value="XL">X-Large</option><option value="XXL">XX-Large</option></select>
- <option value="Cat">Cat</option>
- <option selected="selected" value="Dog">Dog</option>
- <option selected="selected" value="Fish">Fish</option>
- <option value="Bird">Bird</option>
- </select>
```

### ListView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ListView | ⚠️ Divergent | 158 line differences |

#### ListView Diff
```diff
- <table>
+ <table class="table"><thead><tr><td>Id</td>
- <thead>
+ <td>Name</td>
- <tr>
+ <td>Price</td>
- <td>Id</td>
+ <td>Last Update</td></tr></thead>
- <td>Name</td>
+ <tbody><tr><td>1</td>
- <td>Price</td>
+ <td>First Widget</td>
- <td>Last Update</td>
+ <td>$7.99</td>
- </tr>
+ <td>2/26/2026</td></tr><tr class="table-dark"><td>2</td>
- </thead>
+ <td>Second Widget</td>
- <tbody>
+ <td>$13.99</td>
- <tr>
+ <td>2/26/2026</td></tr><tr><td>3</td>
- <td>1</td>
+ <td>Third Widget</td>
- <td>First Widget</td>
+ <td>$100.99</td>
- <td>$7.99</td>
+ <td>2/26/2026</td></tr><tr class="table-dark"><td>4</td>
- <td>2/26/2026</td>
+ <td>Fourth Widget</td>
- </tr>
+ <td>$10.99</td>
- <tr>
+ <td>2/26/2026</td></tr><tr><td>5</td>
- <td colspan="4" style="border-bottom:1px solid #000000;">&nbsp;</td>
+ <td>Fifth Widget</td>
- </tr>
+ <td>$5.99</td>
- <tr class="table-dark">
+ <td>2/26/2026</td></tr><tr class="table-dark"><td>6</td>
... (truncated)
```

### Literal
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Literal-1 | ⚠️ Divergent | Tag structure differs |
| Literal-2 | ⚠️ Divergent | Tag structure differs |
| Literal-3 | ✅ Match | - |

#### Literal-1 Diff
```diff
- This is <b>literal</b> content.
+ <b>Literal</b>
```

#### Literal-2 Diff
```diff
- This is &lt;b&gt;encoded&lt;/b&gt; content.
+ &lt;b&gt;Literal&lt;/b&gt;
```

### Login
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Login-1 | ❌ Missing in source B | File only exists in first directory |
| Login-2 | ❌ Missing in source B | File only exists in first directory |

### LoginName
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| LoginName-1 | ❌ Missing in source B | File only exists in first directory |
| LoginName-2 | ❌ Missing in source B | File only exists in first directory |

### LoginStatus
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| LoginStatus-1 | ❌ Missing in source B | File only exists in first directory |
| LoginStatus-2 | ❌ Missing in source B | File only exists in first directory |

### LoginView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| LoginView-1 | ❌ Missing in source B | File only exists in first directory |

### Menu
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Menu-1 | ❌ Missing in source B | File only exists in first directory |
| Menu-2 | ❌ Missing in source B | File only exists in first directory |
| Menu-3 | ❌ Missing in source B | File only exists in first directory |
| Menu-4 | ❌ Missing in source B | File only exists in first directory |
| Menu-5 | ❌ Missing in source B | File only exists in first directory |
| Menu-6 | ❌ Missing in source B | File only exists in first directory |
| Menu-7 | ❌ Missing in source B | File only exists in first directory |
| Menu-8 | ❌ Missing in source B | File only exists in first directory |
| Menu-9 | ❌ Missing in source B | File only exists in first directory |

### MultiView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| MultiView-1 | ❌ Missing in source B | File only exists in first directory |

### Panel
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Panel-1 | ⚠️ Divergent | Tag structure differs |
| Panel-2 | ⚠️ Divergent | Tag structure differs |
| Panel-3 | ⚠️ Divergent | Tag structure differs |

#### Panel-1 Diff
```diff
- <div id="pnlUserInfo">
+ <div><p>This content is inside a basic Panel.</p>
- <fieldset>
+ <p>A Panel renders as a div element by default.</p></div>
- <legend>
- User Info
- </legend>
- <span id="lblName">Name:</span>
- <input id="txtName" style="" type="text" />
- </fieldset>
- </div>
```

#### Panel-2 Diff
```diff
- <div id="pnlScrollable" style="height:100px; overflow:auto;">
+ <fieldset><legend>User Information</legend>
- <p>First paragraph of content inside the scrollable panel.</p>
+ <p>Name: John Doe</p>
- <p>Second paragraph of content inside the scrollable panel.</p>
+ <p>Email: john@example.com</p></fieldset>
- <p>Third paragraph of content inside the scrollable panel.</p>
- <p>Fourth paragraph of content inside the scrollable panel.</p>
- </div>
```

#### Panel-3 Diff
```diff
- <div id="pnlForm">
+ <div style="background-color:#ffffe0; color:#000080; border:2px solid #0000ff; width:300px;"><p>This panel has custom colors and border.</p></div>
- <input id="txtInput" style="" type="text" />
- <input id="btnSubmit" style="" type="submit" value="Submit" />
- </div>
```

### PasswordRecovery
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| PasswordRecovery-1 | ❌ Missing in source B | File only exists in first directory |
| PasswordRecovery-2 | ❌ Missing in source B | File only exists in first directory |

### PlaceHolder
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| PlaceHolder | ⚠️ Divergent | Tag structure differs |

#### PlaceHolder Diff
```diff
- <p>This content was added programmatically.</p><p>PlaceHolder renders no HTML of its own.</p>
+ <p>This content is inside a PlaceHolder.</p>
+ <p>Note: No extra wrapper element is rendered around this content.</p>
```

### RadioButton
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| RadioButton-1 | ❌ Missing in source B | File only exists in first directory |
| RadioButton-2 | ❌ Missing in source B | File only exists in first directory |
| RadioButton-3 | ❌ Missing in source B | File only exists in first directory |

### RadioButtonList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| RadioButtonList-1 | ⚠️ Divergent | Tag structure differs |
| RadioButtonList-2 | ⚠️ Divergent | Tag structure differs |

#### RadioButtonList-1 Diff
```diff
- <table id="RadioButtonList1">
+ <table><tr><td><input id="f76154fab1ed40358b56d2e460eb4f8e_0" name="f76154fab1ed40358b56d2e460eb4f8e" style="" type="radio" value="red" />
- <tbody><tr>
+ <label for="f76154fab1ed40358b56d2e460eb4f8e_0">Red</label></td></tr><tr><td><input id="f76154fab1ed40358b56d2e460eb4f8e_1" name="f76154fab1ed40358b56d2e460eb4f8e" style="" type="radio" value="green" />
- <td><input id="RadioButtonList1_0" style="" type="radio" value="Excellent" /><label for="MainContent_RadioButtonList1_0">Excellent</label></td>
+ <label for="f76154fab1ed40358b56d2e460eb4f8e_1">Green</label></td></tr><tr><td><input id="f76154fab1ed40358b56d2e460eb4f8e_2" name="f76154fab1ed40358b56d2e460eb4f8e" style="" type="radio" value="blue" />
- </tr><tr>
+ <label for="f76154fab1ed40358b56d2e460eb4f8e_2">Blue</label></td></tr></table>
- <td><input checked="checked" id="RadioButtonList1_1" style="" type="radio" value="Good" /><label for="MainContent_RadioButtonList1_1">Good</label></td>
- </tr><tr>
- <td><input id="RadioButtonList1_2" style="" type="radio" value="Average" /><label for="MainContent_RadioButtonList1_2">Average</label></td>
- </tr><tr>
- <td><input id="RadioButtonList1_3" style="" type="radio" value="Poor" /><label for="MainContent_RadioButtonList1_3">Poor</label></td>
- </tr>
- </tbody></table>
```

#### RadioButtonList-2 Diff
```diff
- <table id="RadioButtonList2">
+ <table><tr><td><input id="759a6aa277b04c639c3e766e9e9a01c4_0" name="759a6aa277b04c639c3e766e9e9a01c4" style="" type="radio" value="S" />
- <tbody><tr>
+ <label for="759a6aa277b04c639c3e766e9e9a01c4_0">Small</label></td><td><input id="759a6aa277b04c639c3e766e9e9a01c4_1" name="759a6aa277b04c639c3e766e9e9a01c4" style="" type="radio" value="M" />
- <td><input id="RadioButtonList2_0" style="" type="radio" value="Yes" /><label for="MainContent_RadioButtonList2_0">Yes</label></td><td><input id="RadioButtonList2_1" style="" type="radio" value="No" /><label for="MainContent_RadioButtonList2_1">No</label></td>
+ <label for="759a6aa277b04c639c3e766e9e9a01c4_1">Medium</label></td><td><input id="759a6aa277b04c639c3e766e9e9a01c4_2" name="759a6aa277b04c639c3e766e9e9a01c4" style="" type="radio" value="L" />
- </tr><tr>
+ <label for="759a6aa277b04c639c3e766e9e9a01c4_2">Large</label></td><td><input id="759a6aa277b04c639c3e766e9e9a01c4_3" name="759a6aa277b04c639c3e766e9e9a01c4" style="" type="radio" value="XL" />
- <td><input id="RadioButtonList2_2" style="" type="radio" value="Maybe" /><label for="MainContent_RadioButtonList2_2">Maybe</label></td><td></td>
+ <label for="759a6aa277b04c639c3e766e9e9a01c4_3">X-Large</label></td></tr></table>
- </tr>
- </tbody></table>
```

### RangeValidator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| RangeValidator-1 | ❌ Missing in source B | File only exists in first directory |
| RangeValidator-2 | ❌ Missing in source B | File only exists in first directory |
| RangeValidator-3 | ❌ Missing in source B | File only exists in first directory |
| RangeValidator-Submit | ❌ Missing in source B | File only exists in first directory |

### RegularExpressionValidator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| RegularExpressionValidator-1 | ❌ Missing in source B | File only exists in first directory |
| RegularExpressionValidator-2 | ❌ Missing in source B | File only exists in first directory |
| RegularExpressionValidator-3 | ❌ Missing in source B | File only exists in first directory |
| RegularExpressionValidator-Submit | ❌ Missing in source B | File only exists in first directory |

### Repeater
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Repeater | ⚠️ Divergent | 62 line differences |

#### Repeater Diff
```diff
- This is a list of widgets
+ <table><tr><td>1</td>
- <li>First Widget</li>
+ <td>First Widget</td>
- <hr/>
+ <td>$7.99</td>
- <li>Second Widget</li>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr class="table-dark"><td>2</td>
- <hr/>
+ <td>Second Widget</td>
- <li>Third Widget</li>
+ <td>$13.99</td>
- <hr/>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr><td>3</td>
- <li>Fourth Widget</li>
+ <td>Third Widget</td>
- <hr/>
+ <td>$100.99</td>
- <li>Fifth Widget</li>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr class="table-dark"><td>4</td>
- <hr/>
+ <td>Fourth Widget</td>
- <li>Sixth Widget</li>
+ <td>$10.99</td>
- <hr/>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr><td>5</td>
- <li>Seventh Widget</li>
+ <td>Fifth Widget</td>
- <hr/>
+ <td>$5.99</td>
- <li>Eighth Widget</li>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr class="table-dark"><td>6</td>
- <hr/>
+ <td>Sixth Widget</td>
- <li>Ninth Widget</li>
+ <td>$6.99</td>
- <hr/>
+ <td>2/26/2026</td></tr><tr><td colspan="4"><hr/></td></tr><tr><td>7</td>
- <li>Tenth Widget</li>
+ <td>Seventh Widget</td>
... (truncated)
```

### RequiredFieldValidator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| RequiredFieldValidator-1 | ❌ Missing in source B | File only exists in first directory |
| RequiredFieldValidator-2 | ❌ Missing in source B | File only exists in first directory |
| RequiredFieldValidator-3 | ❌ Missing in source B | File only exists in first directory |
| RequiredFieldValidator-Submit | ❌ Missing in source B | File only exists in first directory |

### SiteMapPath
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| SiteMapPath-1 | ⚠️ Divergent | Tag structure differs |
| SiteMapPath-2 | ⚠️ Divergent | Tag structure differs |

#### SiteMapPath-1 Diff
```diff
- <span id="SiteMapPath1"><a href="#MainContent_SiteMapPath1_SkipLink" style="position:absolute; left:-10000px; top:auto; width:1px; height:1px; overflow:hidden;">Skip Navigation Links</a><span><a href="/Default.aspx" title="Home Page">Home</a></span><span> &gt; </span><span>SiteMapPath</span><a id="SiteMapPath1_SkipLink"></a></span>
+ <span><a href="/" title="Return to home page">Home</a><span> &gt; </span><a href="/products" title="Browse our product catalog">Products</a><span> &gt; </span><a href="/products/electronics" title="Electronic devices">Electronics</a><span> &gt; </span><span title="Mobile phones">Phones</span></span>
```

#### SiteMapPath-2 Diff
```diff
- <span id="SiteMapPath2"><a href="#MainContent_SiteMapPath2_SkipLink" style="position:absolute; left:-10000px; top:auto; width:1px; height:1px; overflow:hidden;">Skip Navigation Links</a><span><a href="/Default.aspx" style="color:#00008b; font-weight:bold;" title="Home Page">Home</a></span><span style="color:#808080;"> &gt; </span><span style="color:#808080; font-weight:bold;">SiteMapPath</span><a id="SiteMapPath2_SkipLink"></a></span>
+ <span><a href="/" title="Return to home page">Home</a><span> / </span><a href="/products" title="Browse our product catalog">Products</a><span> / </span><a href="/products/electronics" title="Electronic devices">Electronics</a><span> / </span><span title="Mobile phones">Phones</span></span>
```

### Table
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Table-1 | ⚠️ Divergent | Tag structure differs |
| Table-2 | ⚠️ Divergent | Tag structure differs |
| Table-3 | ❌ Missing in source B | File only exists in first directory |

#### Table-1 Diff
```diff
- <table id="Table1" style="border-color:#000000; border-width:1px; border-style:solid; border-collapse:collapse;">
+ <tablecomponent><tablerowcomponent><tableheadercellcomponent>Header 1</tableheadercellcomponent>
- <tbody><tr>
+ <tableheadercellcomponent>Header 2</tableheadercellcomponent>
- <th>Name</th><th>Category</th><th>Price</th>
+ <tableheadercellcomponent>Header 3</tableheadercellcomponent></tablerowcomponent>
- </tr><tr>
+ <tablerowcomponent><tablecellcomponent>Cell 1</tablecellcomponent>
- <td>Widget A</td><td>Hardware</td><td>$9.99</td>
+ <tablecellcomponent>Cell 2</tablecellcomponent>
- </tr><tr>
+ <tablecellcomponent>Cell 3</tablecellcomponent></tablerowcomponent>
- <td>Widget B</td><td>Software</td><td>$19.99</td>
+ <tablerowcomponent><tablecellcomponent>Cell 4</tablecellcomponent>
- </tr><tr>
+ <tablecellcomponent>Cell 5</tablecellcomponent>
- <td>Widget C</td><td>Hardware</td><td>$14.99</td>
+ <tablecellcomponent>Cell 6</tablecellcomponent></tablerowcomponent></tablecomponent>
- </tr>
- </tbody></table>
```

#### Table-2 Diff
```diff
- <table class="styled-table" id="Table2" style="background-color:#ffffcc; border-color:#999999; border-width:1px; border-style:solid;">
+ <tablecomponent caption="Product List"><tablerowcomponent><tableheadercellcomponent>Product</tableheadercellcomponent>
- <tbody><tr style="color:#ffffff; background-color:#5d7b9d;">
+ <tableheadercellcomponent>Price</tableheadercellcomponent></tablerowcomponent>
- <th>ID</th><th>Product</th><th>In Stock</th>
+ <tablerowcomponent><tablecellcomponent>Widget</tablecellcomponent>
- </tr><tr>
+ <tablecellcomponent>$10.00</tablecellcomponent></tablerowcomponent>
- <td>1</td><td>Alpha</td><td>Yes</td>
+ <tablerowcomponent><tablecellcomponent>Gadget</tablecellcomponent>
- </tr><tr style="background-color:#f7f6f3;">
+ <tablecellcomponent>$25.00</tablecellcomponent></tablerowcomponent></tablecomponent>
- <td>2</td><td>Beta</td><td>No</td>
- </tr><tr>
- <td>3</td><td>Gamma</td><td>Yes</td>
- </tr>
- </tbody></table>
```

### TextBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| TextBox-1 | ❌ Missing in source B | File only exists in first directory |
| TextBox-2 | ❌ Missing in source B | File only exists in first directory |
| TextBox-3 | ❌ Missing in source B | File only exists in first directory |
| TextBox-4 | ❌ Missing in source B | File only exists in first directory |
| TextBox-5 | ❌ Missing in source B | File only exists in first directory |
| TextBox-6 | ❌ Missing in source B | File only exists in first directory |
| TextBox-7 | ❌ Missing in source B | File only exists in first directory |

### TreeView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| TreeView | ⚠️ Divergent | 27 line differences |

#### TreeView Diff
```diff
- <a href="#MainContent_SampleTreeView_SkipLink" style="position:absolute; left:-10000px; top:auto; width:1px; height:1px; overflow:hidden;">Skip Navigation Links.</a><div class="Foo" id="SampleTreeView">
+ <div id="SampleTreeView">
- <table style="border-width:0;">
+ <table style="border-width:0;"><tr> <td><a href=""><img alt="Collapse Home" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Home" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a href="/" id="SampleTreeViewt0" target="Content">Home</a></td></tr></table><div><table style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><img alt="" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" /></td><td style="white-space:nowrap;"><a id="SampleTreeViewt0">Foo</a></td></tr></table>
- <tbody><tr>
+ <table style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><a href=""><img alt="Collapse Bar" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Bar" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a id="SampleTreeViewt0">Bar</a></td></tr></table><div><table style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><a href=""><img alt="Collapse Baz" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Baz" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a id="SampleTreeViewt0">Baz</a></td></tr></table><div><table style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><img alt="" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" /></td><td style="white-space:nowrap;"><a id="SampleTreeViewt0">BlazorMisterMagoo</a></td></tr></table></div></div></div>
- <td><a href="Home.aspx" id="SampleTreeViewn0i" tabindex="-1" target="Content"><img alt="This is the home image tooltip" src="../../Content/Images/C#.png" style="border-width:0;" title="This is the home image tooltip" /></a></td><td style="white-space:nowrap;"><input checked="checked" id="SampleTreeViewn0CheckBox" name="MainContent_SampleTreeViewn0CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Home.aspx" id="SampleTreeViewn0" target="Content">Home</a></td>
+ </div>
- </tr>
- </tbody></table><div id="SampleTreeViewn0Nodes" style="display:block;">
- <table style="border-width:0;">
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="SampleTreeViewn1CheckBox" name="MainContent_SampleTreeViewn1CheckBox" style="" title="ToolTop" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Page1.aspx" id="SampleTreeViewn1" target="Content" title="ToolTop">Page1</a></td>
- </tr>
- </tbody></table><div id="SampleTreeViewn1Nodes" style="display:none;">
- <table style="border-width:0;">
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="SampleTreeViewn2CheckBox" name="MainContent_SampleTreeViewn2CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Section1.aspx" id="SampleTreeViewn2" target="Content">Section 1</a></td>
- </tr>
- </tbody></table>
- </div><table style="border-width:0;">
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="SampleTreeViewn3CheckBox" name="MainContent_SampleTreeViewn3CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Page2.aspx" id="SampleTreeViewn3" target="Content">Page 2</a></td>
- </tr>
- </tbody></table>
- </div>
- </div><a id="SampleTreeView_SkipLink"></a>
```

### ValidationSummary
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ValidationSummary-1 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-2 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-3 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-Submit | ❌ Missing in source B | File only exists in first directory |
