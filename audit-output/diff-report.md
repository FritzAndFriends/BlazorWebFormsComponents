# HTML Audit Comparison Report

Generated: 2026-02-26T03:18:37.469Z

## Summary
- Controls compared: 70
- Exact matches: 0
- Divergences found: 70

## Results by Control

### AdRotator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| AdRotator | ⚠️ Divergent | Tag structure differs |

#### AdRotator Diff
```diff
- <a href="https://bing.com" id="MainContent_AdRotator1" target="_top"><img alt="Visit Bing" src="/Content/Images/banner.png" /></a>
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
- <ul id="MainContent_blDisc" style="list-style-type:disc;">
+ <ul><li><span>First item</span></li><li><span>Second item</span></li><li><span>Third item</span></li></ul>
- <li>Apple</li><li>Banana</li><li>Cherry</li><li>Date</li>
- </ul>
```

#### BulletedList-2 Diff
```diff
- <ol id="MainContent_blNumbered" style="list-style-type:decimal;">
+ <ul style="list-style-type:disc;"><li><span>Item One</span></li><li><span>Item Two</span></li><li><span>Item Three</span></li><li><span>Item Four</span></li></ul>
- <li>First</li><li>Second</li><li>Third</li>
- </ol>
```

#### BulletedList-3 Diff
```diff
- <ul id="MainContent_blSquare" style="list-style-type:square;">
+ <ul style="list-style-type:circle;"><li><span>Item One</span></li><li><span>Item Two</span></li><li><span>Item Three</span></li><li><span>Item Four</span></li></ul>
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
- <input id="MainContent_styleButton" style="color:#ffffff; background-color:#0000ff;" type="submit" value="Blue Button" />
+ <button accesskey="b" title="Click to submit" type="submit">Click me!</button>
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
- <table cellpadding="2" cellspacing="0" id="MainContent_Calendar1" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-2 Diff
```diff
- <table cellpadding="2" cellspacing="0" id="MainContent_CalendarDay" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><span>1</span></td><td align="center"><span>2</span></td><td align="center"><span>3</span></td><td align="center"><span>4</span></td><td align="center"><span>5</span></td><td align="center"><span>6</span></td><td align="center"><span>7</span></td></tr><tr><td align="center"><span>8</span></td><td align="center"><span>9</span></td><td align="center"><span>10</span></td><td align="center"><span>11</span></td><td align="center"><span>12</span></td><td align="center"><span>13</span></td><td align="center"><span>14</span></td></tr><tr><td align="center"><span>15</span></td><td align="center"><span>16</span></td><td align="center"><span>17</span></td><td align="center"><span>18</span></td><td align="center"><span>19</span></td><td align="center"><span>20</span></td><td align="center"><span>21</span></td></tr><tr><td align="center"><span>22</span></td><td align="center"><span>23</span></td><td align="center"><span>24</span></td><td align="center"><span>25</span></td><td align="center"><span>26</span></td><td align="center"><span>27</span></td><td align="center"><span>28</span></td></tr><tr><td align="center"><span>1</span></td><td align="center"><span>2</span></td><td align="center"><span>3</span></td><td align="center"><span>4</span></td><td align="center"><span>5</span></td><td align="center"><span>6</span></td><td align="center"><span>7</span></td></tr><tr><td align="center"><span>8</span></td><td align="center"><span>9</span></td><td align="center"><span>10</span></td><td align="center"><span>11</span></td><td align="center"><span>12</span></td><td align="center"><span>13</span></td><td align="center"><span>14</span></td></tr></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-3 Diff
```diff
- <table cellpadding="2" cellspacing="0" id="MainContent_CalendarWeek" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="8" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><td align="center"></td><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-4 Diff
```diff
- <table cellpadding="2" cellspacing="0" id="MainContent_CalendarMonth" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><td></td><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th></th><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="8" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><td align="center"><a href="[postback]" style="color:#000000;" title="Select the whole month">&gt;&gt;</a></td><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:12%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-5 Diff
```diff
- <table cellpadding="4" cellspacing="0" id="MainContent_CalendarStyled" rules="all" style="color:#000000; background-color:#ffffff; border-color:#999999; border-width:1px; border-style:solid; font-family:verdana; font-size:8pt; width:220px; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer;">&gt;&gt;</a></td><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th></th><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">&gt;&gt;</a></td><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="7" style="background-color:#ccccff;"><table cellspacing="0" style="color:#333399; font-family:verdana; font-size:13pt; font-weight:bold; width:100%; border-collapse:collapse;">
- <tbody><tr><td style="color:#333399; font-size:8pt; width:15%;" valign="bottom"><a href="[postback]" style="color:#333399;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="color:#333399; font-size:8pt; width:15%;" valign="bottom"><a href="[postback]" style="color:#333399;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Su</th><th abbr="Monday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Mo</th><th abbr="Tuesday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Tu</th><th abbr="Wednesday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">We</th><th abbr="Thursday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Th</th><th abbr="Friday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Fr</th><th abbr="Saturday" align="center" scope="col" style="background-color:#ccccff; font-size:7pt; font-weight:bold;">Sa</th></tr><tr><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#999999;" title="January 25">25</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="January 26">26</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="January 27">27</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="January 28">28</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="January 29">29</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="January 30">30</a></td><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#999999;" title="January 31">31</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="color:#000000; background-color:#ccccff; width:14%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#999999;" title="March 1">1</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="March 2">2</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="March 3">3</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="March 4">4</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="March 5">5</a></td><td align="center" style="color:#999999; width:14%;"><a href="[postback]" style="color:#999999;" title="March 6">6</a></td><td align="center" style="color:#999999; background-color:#ffffcc; width:14%;"><a href="[postback]" style="color:#999999;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-6 Diff
```diff
- <table cellpadding="2" cellspacing="0" id="MainContent_CalendarNav" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table border="1" cellpadding="2" cellspacing="0" style="border-collapse:collapse;"><tr><td><a style="cursor:pointer;">&lt;</a></td><td align="center" colspan="5">February 2026</td><td><a style="cursor:pointer;">&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">« Prev</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">Next »</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

#### Calendar-7 Diff
```diff
- <table cellpadding="2" cellspacing="0" id="MainContent_CalendarEvents" style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">
+ <table cellpadding="2" cellspacing="0"><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr><tr><td align="center"><a style="cursor:pointer;">15</a></td><td align="center"><a style="cursor:pointer;">16</a></td><td align="center"><a style="cursor:pointer;">17</a></td><td align="center"><a style="cursor:pointer;">18</a></td><td align="center"><a style="cursor:pointer;">19</a></td><td align="center"><a style="cursor:pointer;">20</a></td><td align="center"><a style="cursor:pointer;">21</a></td></tr><tr><td align="center"><a style="cursor:pointer;">22</a></td><td align="center"><a style="cursor:pointer;">23</a></td><td align="center"><a style="cursor:pointer;">24</a></td><td align="center"><a style="cursor:pointer;">25</a></td><td align="center"><a style="cursor:pointer;">26</a></td><td align="center"><a style="cursor:pointer;">27</a></td><td align="center"><a style="cursor:pointer;">28</a></td></tr><tr><td align="center"><a style="cursor:pointer;">1</a></td><td align="center"><a style="cursor:pointer;">2</a></td><td align="center"><a style="cursor:pointer;">3</a></td><td align="center"><a style="cursor:pointer;">4</a></td><td align="center"><a style="cursor:pointer;">5</a></td><td align="center"><a style="cursor:pointer;">6</a></td><td align="center"><a style="cursor:pointer;">7</a></td></tr><tr><td align="center"><a style="cursor:pointer;">8</a></td><td align="center"><a style="cursor:pointer;">9</a></td><td align="center"><a style="cursor:pointer;">10</a></td><td align="center"><a style="cursor:pointer;">11</a></td><td align="center"><a style="cursor:pointer;">12</a></td><td align="center"><a style="cursor:pointer;">13</a></td><td align="center"><a style="cursor:pointer;">14</a></td></tr></table>
- <tbody><tr><td colspan="7" style="background-color:#c0c0c0;"><table cellspacing="0" style="width:100%; border-collapse:collapse;">
- <tbody><tr><td style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="[postback]" style="color:#000000;" title="Go to the next month">&gt;</a></td></tr>
- </tbody></table></td></tr><tr><th abbr="Sunday" align="center" scope="col">Sun</th><th abbr="Monday" align="center" scope="col">Mon</th><th abbr="Tuesday" align="center" scope="col">Tue</th><th abbr="Wednesday" align="center" scope="col">Wed</th><th abbr="Thursday" align="center" scope="col">Thu</th><th abbr="Friday" align="center" scope="col">Fri</th><th abbr="Saturday" align="center" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="[postback]" style="color:#000000;" title="March 7">7</a></td></tr>
- </tbody></table>
```

### CheckBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CheckBox-1 | ⚠️ Divergent | Tag structure differs |
| CheckBox-2 | ⚠️ Divergent | Tag structure differs |
| CheckBox-3 | ⚠️ Divergent | Tag structure differs |

#### CheckBox-1 Diff
```diff
- <input id="MainContent_chkTerms" style="" type="checkbox" /><label for="MainContent_chkTerms">Accept Terms</label>
+ <span><input id="1c0d196d71644cf1b391bc8a93964e15" style="" type="checkbox" />
+ <label for="1c0d196d71644cf1b391bc8a93964e15">I agree to terms</label></span>
```

#### CheckBox-2 Diff
```diff
- <input checked="checked" id="MainContent_chkSubscribe" style="" type="checkbox" /><label for="MainContent_chkSubscribe">Subscribe</label>
+ <span><input id="94721a91a32b40cd8df2ae695c41fb29" style="" type="checkbox" />
+ <label for="94721a91a32b40cd8df2ae695c41fb29">Already checked</label></span>
```

#### CheckBox-3 Diff
```diff
- <input id="MainContent_chkFeature" onclick="javascript:setTimeout('__doPostBack(\'ctl00$MainContent$chkFeature\',\'\')', 0)" style="" type="checkbox" /><label for="MainContent_chkFeature">Enable Feature</label>
+ <span><input id="06148c0d2876488783ab762489e109cb" style="" type="checkbox" />
+ <label for="06148c0d2876488783ab762489e109cb">Label on right (default)</label></span>
```

### CheckBoxList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CheckBoxList-1 | ❌ Missing in source B | File only exists in first directory |
| CheckBoxList-2 | ❌ Missing in source B | File only exists in first directory |

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
- <select id="MainContent_ddlStatic">
+ <select><option selected="" value="">Select...</option><option value="1">Option One</option><option value="2">Option Two</option><option value="3">Option Three</option></select>
- <option value="">Select...</option>
- <option value="1">Option One</option>
- <option value="2">Option Two</option>
- <option value="3">Option Three</option>
- </select>
```

#### DropDownList-2 Diff
```diff
- <select id="MainContent_ddlSelected">
+ <select><option value="apple">Apple</option><option selected="" value="banana">Banana</option><option value="cherry">Cherry</option></select>
- <option value="apple">Apple</option>
- <option selected="selected" value="banana">Banana</option>
- <option value="cherry">Cherry</option>
- </select>
```

#### DropDownList-3 Diff
```diff
- <select id="MainContent_ddlDataBound">
+ <select><option value="1">Widget</option><option value="2">Gadget</option><option value="3">Gizmo</option><option value="4">Doohickey</option></select>
- <option value="1">First Item</option>
- <option value="2">Second Item</option>
- <option value="3">Third Item</option>
- </select>
```

#### DropDownList-4 Diff
```diff
- <select class="aspNetDisabled" disabled="disabled" id="MainContent_ddlDisabled">
+ <select><option value="1">9.99</option><option value="2">24.50</option><option value="3">149.95</option><option value="4">3.00</option></select>
- <option selected="selected" value="1">Cannot change</option>
- </select>
```

#### DropDownList-5 Diff
```diff
- <select class="form-select" id="MainContent_ddlStyled">
+ <select><option value="1">Item: Widget</option><option value="2">Item: Gadget</option><option value="3">Item: Gizmo</option><option value="4">Item: Doohickey</option></select>
- <option value="1">Styled</option>
- </select>
```

#### DropDownList-6 Diff
```diff
- <select id="MainContent_ddlColors" style="color:#000080; background-color:#ffffe0; width:200px;">
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
- <input id="MainContent_FileUpload1" style="" type="file" />
+ <input c2a4-44c1-86ca-61cac40f7a4a="" style="" type="file" />
- <input id="MainContent_btnUpload" style="" type="submit" value="Upload" />
```

### HiddenField
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| HiddenField | ⚠️ Divergent | Tag structure differs |

#### HiddenField Diff
```diff
- <input id="MainContent_HiddenField1" style="" type="hidden" value="secret-value-123" />
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
- <a href="https://bing.com" id="MainContent_styleLink" style="color:#ffffff; background-color:#0000ff;">Blue Button</a>
+ <a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a href="https://bing.com" id="MainContent_HyperLink1" title="Navigate to Bing!">Blue Button</a>
+ <a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a href="https://bing.com" id="MainContent_HyperLink3">Blue Button</a>
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
- <a href="https://bing.com" id="MainContent_styleLink" style="color:#ffffff; background-color:#0000ff;">Blue Button</a>
+ <a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a href="https://bing.com" id="MainContent_HyperLink1" title="Navigate to Bing!">Blue Button</a>
+ <a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a href="https://bing.com" id="MainContent_HyperLink3">Blue Button</a>
+ <a href="https://www.github.com" style="background-color:#696969; color:#ffffff;" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

### Image
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Image-1 | ⚠️ Divergent | Tag structure differs |
| Image-2 | ⚠️ Divergent | Tag structure differs |

#### Image-1 Diff
```diff
- <img alt="Banner image" id="MainContent_imgBasic" src="../../Content/Images/banner.png" />
+ <img alt="Sample placeholder image" longdesc="" src="/img/placeholder-150x100.svg" />
```

#### Image-2 Diff
```diff
- <img alt="Sized image" id="MainContent_imgSized" src="../../Content/Images/banner.png" style="height:100px; width:200px;" />
+ <img alt="Image with tooltip" longdesc="" src="/img/placeholder-150x100.svg" title="This is a tooltip displayed on hover" />
```

### ImageButton
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ImageButton-1 | ❌ Missing in source B | File only exists in first directory |
| ImageButton-2 | ❌ Missing in source B | File only exists in first directory |

### ImageMap
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ImageMap | ⚠️ Divergent | Tag structure differs |

#### ImageMap Diff
```diff
- <img alt="Navigate" id="MainContent_ImageMap1" src="../../Content/Images/banner.png" usemap="#ImageMapMainContent_ImageMap1" /><map id="ImageMapMainContent_ImageMap1" name="ImageMapMainContent_ImageMap1">
+ <img alt="Navigation demo image" src="/img/placeholder-400x200.svg" usemap="#ImageMap_a144b249bb8440c886fcdb4194f3d9c0" /><map name="ImageMap_a144b249bb8440c886fcdb4194f3d9c0"><area alt="Go to Button samples" coords="0,0,130,200" href="/ControlSamples/Button" shape="rect" /><area alt="Go to CheckBox samples" coords="200,100,60" href="/ControlSamples/CheckBox" shape="circle" /><area alt="Go to Image samples" coords="270,0,400,200" href="/ControlSamples/Image" shape="rect" /></map>
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
- <span id="MainContent_lblBasic">Hello World</span>
+ <span>Hello, World!</span>
```

#### Label-2 Diff
```diff
- <span class="text-primary" id="MainContent_lblStyled" style="color:#0000ff; font-weight:bold;">Styled Label</span>
+ <span class="text-danger fw-bold" style="color:#ff0000;">Important notice</span>
```

#### Label-3 Diff
```diff
- <span id="MainContent_lblHtml"><em>Emphasized</em></span>
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
- <a class="btn btn-primary" href="[postback]" id="MainContent_LinkButton1">Click Me</a>
+ <a>LinkButton1 with Command</a>
```

#### LinkButton-2 Diff
```diff
- <a href="[postback]" id="MainContent_LinkButton2">Submit Form</a>
+ <a>LinkButton2 with Command</a>
```

#### LinkButton-3 Diff
```diff
- <a class="aspNetDisabled" id="MainContent_LinkButton3">Disabled Link</a>
+ <a href="ControlSamples/LinkButton/#">Click me!!</a>
```

### ListBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ListBox-1 | ❌ Missing in source B | File only exists in first directory |
| ListBox-2 | ❌ Missing in source B | File only exists in first directory |

### Literal
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Literal-1 | ⚠️ Divergent | Tag structure differs |
| Literal-2 | ⚠️ Divergent | Tag structure differs |
| Literal-3 | ❌ Missing in source B | File only exists in first directory |

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

### Panel
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Panel-1 | ⚠️ Divergent | Tag structure differs |
| Panel-2 | ⚠️ Divergent | Tag structure differs |
| Panel-3 | ⚠️ Divergent | Tag structure differs |

#### Panel-1 Diff
```diff
- <div id="MainContent_pnlUserInfo">
+ <div><p>This content is inside a basic Panel.</p>
- <fieldset>
+ <p>A Panel renders as a div element by default.</p></div>
- <legend>
- User Info
- </legend>
- <span id="MainContent_lblName">Name:</span>
- <input id="MainContent_txtName" style="" type="text" />
- </fieldset>
- </div>
```

#### Panel-2 Diff
```diff
- <div id="MainContent_pnlScrollable" style="height:100px; overflow:auto;">
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
- <div id="MainContent_pnlForm" onkeypress="javascript:return WebForm_FireDefaultButton(event, 'MainContent_btnSubmit')">
+ <div style="background-color:#ffffe0; color:#000080; border:2px solid #0000ff; width:300px;"><p>This panel has custom colors and border.</p></div>
- <input id="MainContent_txtInput" style="" type="text" />
- <input id="MainContent_btnSubmit" style="" type="submit" value="Submit" />
- </div>
```

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
- <table id="MainContent_RadioButtonList1">
+ <table><tr><td><input id="13a41b7187904134849d890a4407a89d_0" name="13a41b7187904134849d890a4407a89d" style="" type="radio" value="red" />
- <tbody><tr>
+ <label for="13a41b7187904134849d890a4407a89d_0">Red</label></td></tr><tr><td><input id="13a41b7187904134849d890a4407a89d_1" name="13a41b7187904134849d890a4407a89d" style="" type="radio" value="green" />
- <td><input id="MainContent_RadioButtonList1_0" style="" type="radio" value="Excellent" /><label for="MainContent_RadioButtonList1_0">Excellent</label></td>
+ <label for="13a41b7187904134849d890a4407a89d_1">Green</label></td></tr><tr><td><input id="13a41b7187904134849d890a4407a89d_2" name="13a41b7187904134849d890a4407a89d" style="" type="radio" value="blue" />
- </tr><tr>
+ <label for="13a41b7187904134849d890a4407a89d_2">Blue</label></td></tr></table>
- <td><input checked="checked" id="MainContent_RadioButtonList1_1" style="" type="radio" value="Good" /><label for="MainContent_RadioButtonList1_1">Good</label></td>
- </tr><tr>
- <td><input id="MainContent_RadioButtonList1_2" style="" type="radio" value="Average" /><label for="MainContent_RadioButtonList1_2">Average</label></td>
- </tr><tr>
- <td><input id="MainContent_RadioButtonList1_3" style="" type="radio" value="Poor" /><label for="MainContent_RadioButtonList1_3">Poor</label></td>
- </tr>
- </tbody></table>
```

#### RadioButtonList-2 Diff
```diff
- <table id="MainContent_RadioButtonList2">
+ <table><tr><td><input id="a04ef17133f7436c8468915ba3bd9d79_0" name="a04ef17133f7436c8468915ba3bd9d79" style="" type="radio" value="S" />
- <tbody><tr>
+ <label for="a04ef17133f7436c8468915ba3bd9d79_0">Small</label></td><td><input id="a04ef17133f7436c8468915ba3bd9d79_1" name="a04ef17133f7436c8468915ba3bd9d79" style="" type="radio" value="M" />
- <td><input id="MainContent_RadioButtonList2_0" style="" type="radio" value="Yes" /><label for="MainContent_RadioButtonList2_0">Yes</label></td><td><input id="MainContent_RadioButtonList2_1" style="" type="radio" value="No" /><label for="MainContent_RadioButtonList2_1">No</label></td>
+ <label for="a04ef17133f7436c8468915ba3bd9d79_1">Medium</label></td><td><input id="a04ef17133f7436c8468915ba3bd9d79_2" name="a04ef17133f7436c8468915ba3bd9d79" style="" type="radio" value="L" />
- </tr><tr>
+ <label for="a04ef17133f7436c8468915ba3bd9d79_2">Large</label></td><td><input id="a04ef17133f7436c8468915ba3bd9d79_3" name="a04ef17133f7436c8468915ba3bd9d79" style="" type="radio" value="XL" />
- <td><input id="MainContent_RadioButtonList2_2" style="" type="radio" value="Maybe" /><label for="MainContent_RadioButtonList2_2">Maybe</label></td><td></td>
+ <label for="a04ef17133f7436c8468915ba3bd9d79_3">X-Large</label></td></tr></table>
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
| TreeView | ⚠️ Divergent | 30 line differences |

#### TreeView Diff
```diff
- <a href="#MainContent_SampleTreeView_SkipLink" style="position:absolute; left:-10000px; top:auto; width:1px; height:1px; overflow:hidden;">Skip Navigation Links.</a><div class="Foo" id="MainContent_SampleTreeView">
+ <div id="SampleTreeView">
- <table cellpadding="0" cellspacing="0" style="border-width:0;">
+ <table cellpadding="0" cellspacing="0" style="border-width:0;"><tr> <td><a href=""><img alt="Collapse Home" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Home" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a href="/" id="MainContent_SampleTreeViewt0" target="Content">Home</a></td></tr></table>
- <tbody><tr>
+ <table cellpadding="0" cellspacing="0" style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><img alt="" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" /></td><td style="white-space:nowrap;"><a id="MainContent_SampleTreeViewt0">Foo</a></td></tr></table>
- <td><a href="Home.aspx" id="MainContent_SampleTreeViewn0i" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn0');" tabindex="-1" target="Content"><img alt="This is the home image tooltip" src="../../Content/Images/C#.png" style="border-width:0;" title="This is the home image tooltip" /></a></td><td style="white-space:nowrap;"><input checked="checked" id="MainContent_SampleTreeViewn0CheckBox" name="MainContent_SampleTreeViewn0CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Home.aspx" id="MainContent_SampleTreeViewn0" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn0');" target="Content">Home</a></td>
+ <table cellpadding="0" cellspacing="0" style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><a href=""><img alt="Collapse Bar" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Bar" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a id="MainContent_SampleTreeViewt0">Bar</a></td></tr></table>
- </tr>
+ <table cellpadding="0" cellspacing="0" style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><a href=""><img alt="Collapse Baz" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" style="border-width:0;" title="Collapse Baz" /></a></td><td style="white-space:nowrap;"><input style="" type="checkbox" /><a id="MainContent_SampleTreeViewt0">Baz</a></td></tr></table>
- </tbody></table><div id="MainContent_SampleTreeViewn0Nodes" style="display:block;">
+ <table cellpadding="0" cellspacing="0" style="border-width:0;"><tr><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td><img alt="" src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" /></td><td style="white-space:nowrap;"><a id="MainContent_SampleTreeViewt0">BlazorMisterMagoo</a></td></tr></table>
- <table cellpadding="0" cellspacing="0" style="border-width:0;">
+ </div>
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="MainContent_SampleTreeViewn1CheckBox" name="MainContent_SampleTreeViewn1CheckBox" style="" title="ToolTop" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Page1.aspx" id="MainContent_SampleTreeViewn1" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn1');" target="Content" title="ToolTop">Page1</a></td>
- </tr>
- </tbody></table><div id="MainContent_SampleTreeViewn1Nodes" style="display:none;">
- <table cellpadding="0" cellspacing="0" style="border-width:0;">
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="MainContent_SampleTreeViewn2CheckBox" name="MainContent_SampleTreeViewn2CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Section1.aspx" id="MainContent_SampleTreeViewn2" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn2');" target="Content">Section 1</a></td>
- </tr>
- </tbody></table>
- </div><table cellpadding="0" cellspacing="0" style="border-width:0;">
- <tbody><tr>
- <td><div style="width:20px; height:1px;"></div></td><td style="white-space:nowrap;"><input id="MainContent_SampleTreeViewn3CheckBox" name="MainContent_SampleTreeViewn3CheckBox" style="" type="checkbox" /><a class="MainContent_SampleTreeView_0" href="Page2.aspx" id="MainContent_SampleTreeViewn3" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn3');" target="Content">Page 2</a></td>
- </tr>
- </tbody></table>
- </div>
- </div><a id="MainContent_SampleTreeView_SkipLink"></a>
```
