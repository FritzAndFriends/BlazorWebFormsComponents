# HTML Audit Comparison Report

Generated: 2026-02-26T05:02:22.620Z

## Summary
- Controls compared: 132
- Exact matches: 0
- Divergences found: 132

## Results by Control

### AdRotator
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| AdRotator | ⚠️ Divergent | Tag structure differs |

#### AdRotator Diff
```diff
- <a id="MainContent_AdRotator1" href="https://bing.com" target="_top"><img src="/Content/Images/banner.png" alt="Visit Bing"></a>
+ <!--!--><!--!--><a href="http://www.microsoft.com" target="_top"><img src="/img/VB.png" width="397" height="343" alt="Visual Basic"></a>
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
+ <!--!--><!--!--><ul><li><span>First item</span></li><li><span>Second item</span></li><li><span>Third item</span></li></ul>
- 	<li>Apple</li><li>Banana</li><li>Cherry</li><li>Date</li>
- </ul>
```

#### BulletedList-2 Diff
```diff
- <ol id="MainContent_blNumbered" style="list-style-type:decimal;">
+ <!--!--><!--!--><ul style="list-style-type: disc;"><li><span>Item One</span></li><li><span>Item Two</span></li><li><span>Item Three</span></li><li><span>Item Four</span></li></ul>
- 	<li>First</li><li>Second</li><li>Third</li>
- </ol>
```

#### BulletedList-3 Diff
```diff
- <ul id="MainContent_blSquare" style="list-style-type:square;">
+ <!--!--><!--!--><ul style="list-style-type: circle;"><li><span>Item One</span></li><li><span>Item Two</span></li><li><span>Item Three</span></li><li><span>Item Four</span></li></ul>
- 	<li><a href="https://example.com">Example Site</a></li><li><a href="https://example.org">Example Org</a></li>
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
- <input type="submit" name="ctl00$MainContent$styleButton" value="Blue Button" id="MainContent_styleButton" style="color: white; background-color: blue;">
+ <!--!--><!--!--><button type="submit" title="Click to submit" accesskey="b">Click me!</button>
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
- <table id="MainContent_Calendar1" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="7" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','V9497')" style="color:Black" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','V9556')" style="color:Black" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$Calendar1','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
```

#### Calendar-2 Diff
```diff
- <table id="MainContent_CalendarDay" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="7" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','V9497')" style="color:Black" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','V9556')" style="color:Black" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarDay','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><span>1</span></td><td align="center"><span>2</span></td><td align="center"><span>3</span></td><td align="center"><span>4</span></td><td align="center"><span>5</span></td><td align="center"><span>6</span></td><td align="center"><span>7</span></td></tr><tr><td align="center"><span>8</span></td><td align="center"><span>9</span></td><td align="center"><span>10</span></td><td align="center"><span>11</span></td><td align="center"><span>12</span></td><td align="center"><span>13</span></td><td align="center"><span>14</span></td></tr><tr><td align="center"><span>15</span></td><td align="center"><span>16</span></td><td align="center"><span>17</span></td><td align="center"><span>18</span></td><td align="center"><span>19</span></td><td align="center"><span>20</span></td><td align="center"><span>21</span></td></tr><tr><td align="center"><span>22</span></td><td align="center"><span>23</span></td><td align="center"><span>24</span></td><td align="center"><span>25</span></td><td align="center"><span>26</span></td><td align="center"><span>27</span></td><td align="center"><span>28</span></td></tr><tr><td align="center"><span>1</span></td><td align="center"><span>2</span></td><td align="center"><span>3</span></td><td align="center"><span>4</span></td><td align="center"><span>5</span></td><td align="center"><span>6</span></td><td align="center"><span>7</span></td></tr><tr><td align="center"><span>8</span></td><td align="center"><span>9</span></td><td align="center"><span>10</span></td><td align="center"><span>11</span></td><td align="center"><span>12</span></td><td align="center"><span>13</span></td><td align="center"><span>14</span></td></tr></table>
```

#### Calendar-3 Diff
```diff
- <table id="MainContent_CalendarWeek" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="8" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','V9497')" style="color:Black" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','V9556')" style="color:Black" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><td align="center"></td><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R952107')" style="color:Black" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R952807')" style="color:Black" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R953507')" style="color:Black" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R954207')" style="color:Black" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R954907')" style="color:Black" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','R955607')" style="color:Black" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarWeek','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
```

#### Calendar-4 Diff
```diff
- <table id="MainContent_CalendarMonth" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="8" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','V9497')" style="color:Black" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','V9556')" style="color:Black" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><td align="center"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R952828')" style="color:Black" title="Select the whole month">&gt;&gt;</a></td><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R952107')" style="color:Black" title="Select week 1">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R952807')" style="color:Black" title="Select week 2">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R953507')" style="color:Black" title="Select week 3">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R954207')" style="color:Black" title="Select week 4">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R954907')" style="color:Black" title="Select week 5">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','R955607')" style="color:Black" title="Select week 6">&gt;</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:12%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarMonth','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><td></td><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th></th><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
```

#### Calendar-5 Diff
```diff
- <table id="MainContent_CalendarStyled" cellspacing="0" cellpadding="4" rules="all" title="Calendar" style="color:Black;background-color:White;border-color:#999999;border-width:1px;border-style:solid;font-family:Verdana;font-size:8pt;width:220px;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="7" style="background-color:#CCCCFF;"><table cellspacing="0" style="color:#333399;font-family:Verdana;font-size:13pt;font-weight:bold;width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td valign="bottom" style="color:#333399;font-size:8pt;width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','V9497')" style="color:#333399" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" valign="bottom" style="color:#333399;font-size:8pt;width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','V9556')" style="color:#333399" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><th align="center" abbr="Sunday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Su</th><th align="center" abbr="Monday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Mo</th><th align="center" abbr="Tuesday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Tu</th><th align="center" abbr="Wednesday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">We</th><th align="center" abbr="Thursday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Th</th><th align="center" abbr="Friday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Fr</th><th align="center" abbr="Saturday" scope="col" style="background-color:#CCCCFF;font-size:7pt;font-weight:bold;">Sa</th></tr><tr><td align="center" style="color:#999999;background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9521')" style="color:#999999" title="January 25">25</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9522')" style="color:#999999" title="January 26">26</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9523')" style="color:#999999" title="January 27">27</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9524')" style="color:#999999" title="January 28">28</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9525')" style="color:#999999" title="January 29">29</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9526')" style="color:#999999" title="January 30">30</a></td><td align="center" style="color:#999999;background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9527')" style="color:#999999" title="January 31">31</a></td></tr><tr><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="color:Black;background-color:#CCCCFF;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="color:#999999;background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9556')" style="color:#999999" title="March 1">1</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9557')" style="color:#999999" title="March 2">2</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9558')" style="color:#999999" title="March 3">3</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9559')" style="color:#999999" title="March 4">4</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9560')" style="color:#999999" title="March 5">5</a></td><td align="center" style="color:#999999;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9561')" style="color:#999999" title="March 6">6</a></td><td align="center" style="color:#999999;background-color:#FFFFCC;width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarStyled','9562')" style="color:#999999" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><td><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th></th><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer"><!--!-->&gt;&gt;</a></td><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
```

#### Calendar-6 Diff
```diff
- <table id="MainContent_CalendarNav" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="7" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','V9497')" style="color:Black" title="Go to the previous month">« Prev</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','V9556')" style="color:Black" title="Go to the next month">Next »</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarNav','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table style="border-collapse:collapse;" cellpadding="2" cellspacing="0" border="1"><tr><td><a style="cursor:pointer"><!--!-->&lt;</a></td><td colspan="5" align="center">February 2026</td><td><a style="cursor:pointer"><!--!-->&gt;</a></td></tr><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
```

#### Calendar-7 Diff
```diff
- <table id="MainContent_CalendarEvents" cellspacing="0" cellpadding="2" title="Calendar" style="border-width:1px;border-style:solid;border-collapse:collapse;">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<tbody><tr><td colspan="7" style="background-color:Silver;"><table cellspacing="0" style="width:100%;border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr><td style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','V9497')" style="color:Black" title="Go to the previous month">&lt;</a></td><td align="center" style="width:70%;">February 2026</td><td align="right" style="width:15%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','V9556')" style="color:Black" title="Go to the next month">&gt;</a></td></tr>
+ 	<!--!-->
- 	</tbody></table></td></tr><tr><th align="center" abbr="Sunday" scope="col">Sun</th><th align="center" abbr="Monday" scope="col">Mon</th><th align="center" abbr="Tuesday" scope="col">Tue</th><th align="center" abbr="Wednesday" scope="col">Wed</th><th align="center" abbr="Thursday" scope="col">Thu</th><th align="center" abbr="Friday" scope="col">Fri</th><th align="center" abbr="Saturday" scope="col">Sat</th></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9521')" style="color:Black" title="January 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9522')" style="color:Black" title="January 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9523')" style="color:Black" title="January 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9524')" style="color:Black" title="January 28">28</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9525')" style="color:Black" title="January 29">29</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9526')" style="color:Black" title="January 30">30</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9527')" style="color:Black" title="January 31">31</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9528')" style="color:Black" title="February 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9529')" style="color:Black" title="February 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9530')" style="color:Black" title="February 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9531')" style="color:Black" title="February 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9532')" style="color:Black" title="February 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9533')" style="color:Black" title="February 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9534')" style="color:Black" title="February 7">7</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9535')" style="color:Black" title="February 8">8</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9536')" style="color:Black" title="February 9">9</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9537')" style="color:Black" title="February 10">10</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9538')" style="color:Black" title="February 11">11</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9539')" style="color:Black" title="February 12">12</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9540')" style="color:Black" title="February 13">13</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9541')" style="color:Black" title="February 14">14</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9542')" style="color:Black" title="February 15">15</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9543')" style="color:Black" title="February 16">16</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9544')" style="color:Black" title="February 17">17</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9545')" style="color:Black" title="February 18">18</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9546')" style="color:Black" title="February 19">19</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9547')" style="color:Black" title="February 20">20</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9548')" style="color:Black" title="February 21">21</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9549')" style="color:Black" title="February 22">22</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9550')" style="color:Black" title="February 23">23</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9551')" style="color:Black" title="February 24">24</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9552')" style="color:Black" title="February 25">25</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9553')" style="color:Black" title="February 26">26</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9554')" style="color:Black" title="February 27">27</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9555')" style="color:Black" title="February 28">28</a></td></tr><tr><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9556')" style="color:Black" title="March 1">1</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9557')" style="color:Black" title="March 2">2</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9558')" style="color:Black" title="March 3">3</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9559')" style="color:Black" title="March 4">4</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9560')" style="color:Black" title="March 5">5</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9561')" style="color:Black" title="March 6">6</a></td><td align="center" style="width:14%;"><a href="javascript:__doPostBack('ctl00$MainContent$CalendarEvents','9562')" style="color:Black" title="March 7">7</a></td></tr>
+ 	<!--!-->
- </tbody></table>
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<!--!-->
+ 	<table cellpadding="2" cellspacing="0"><tr><th abbr="Sun" scope="col">Sun</th><th abbr="Mon" scope="col">Mon</th><th abbr="Tue" scope="col">Tue</th><th abbr="Wed" scope="col">Wed</th><th abbr="Thu" scope="col">Thu</th><th abbr="Fri" scope="col">Fri</th><th abbr="Sat" scope="col">Sat</th></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr><tr><td align="center"><a style="cursor:pointer">15</a></td><td align="center"><a style="cursor:pointer">16</a></td><td align="center"><a style="cursor:pointer">17</a></td><td align="center"><a style="cursor:pointer">18</a></td><td align="center"><a style="cursor:pointer">19</a></td><td align="center"><a style="cursor:pointer">20</a></td><td align="center"><a style="cursor:pointer">21</a></td></tr><tr><td align="center"><a style="cursor:pointer">22</a></td><td align="center"><a style="cursor:pointer">23</a></td><td align="center"><a style="cursor:pointer">24</a></td><td align="center"><a style="cursor:pointer">25</a></td><td align="center"><a style="cursor:pointer">26</a></td><td align="center"><a style="cursor:pointer">27</a></td><td align="center"><a style="cursor:pointer">28</a></td></tr><tr><td align="center"><a style="cursor:pointer">1</a></td><td align="center"><a style="cursor:pointer">2</a></td><td align="center"><a style="cursor:pointer">3</a></td><td align="center"><a style="cursor:pointer">4</a></td><td align="center"><a style="cursor:pointer">5</a></td><td align="center"><a style="cursor:pointer">6</a></td><td align="center"><a style="cursor:pointer">7</a></td></tr><tr><td align="center"><a style="cursor:pointer">8</a></td><td align="center"><a style="cursor:pointer">9</a></td><td align="center"><a style="cursor:pointer">10</a></td><td align="center"><a style="cursor:pointer">11</a></td><td align="center"><a style="cursor:pointer">12</a></td><td align="center"><a style="cursor:pointer">13</a></td><td align="center"><a style="cursor:pointer">14</a></td></tr></table>
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
- <input id="MainContent_chkTerms" type="checkbox" name="ctl00$MainContent$chkTerms" style=""><label for="MainContent_chkTerms">Accept Terms</label>
+ <!--!--><!--!--><span><input id="ef5926ffb4c04464bce53f64caa55746" type="checkbox" style=""><!--!-->
+ 				<label for="ef5926ffb4c04464bce53f64caa55746">I agree to terms</label></span>
```

#### CheckBox-2 Diff
```diff
- <input id="MainContent_chkSubscribe" type="checkbox" name="ctl00$MainContent$chkSubscribe" checked="checked" style=""><label for="MainContent_chkSubscribe">Subscribe</label>
+ <!--!--><!--!--><span><input id="9c0f7fb07435484db976159a5f9b3cef" type="checkbox" style=""><!--!-->
+ 				<label for="9c0f7fb07435484db976159a5f9b3cef">Already checked</label></span>
```

#### CheckBox-3 Diff
```diff
- <input id="MainContent_chkFeature" type="checkbox" name="ctl00$MainContent$chkFeature" onclick="javascript:setTimeout('__doPostBack(\'ctl00$MainContent$chkFeature\',\'\')', 0)" style=""><label for="MainContent_chkFeature">Enable Feature</label>
+ <!--!--><!--!--><span><input id="2616d6b9ea7241c98b74174e2faa5729" type="checkbox" style=""><!--!-->
+ 				<label for="2616d6b9ea7241c98b74174e2faa5729">Label on right (default)</label></span>
```

### CheckBoxList
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| CheckBoxList-1 | ❌ Missing in source B | File only exists in first directory |
| CheckBoxList-2 | ❌ Missing in source B | File only exists in first directory |

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
| DataList | ⚠️ Divergent | 110 line differences |

#### DataList Diff
```diff
- <table id="MainContent_simpleDataList" tabindex="1" title="This is my tooltip" cellspacing="3" cellpadding="2" itemtype="SharedSampleObjects.Models.Widget">
+ <!--!--><!--!--><!--!--><!--!-->
- 	<caption align="Top">
+ 		<!--!-->
- 		This is my caption
+ 		<!--!--><!--!-->
- 	</caption><tbody><tr>
+ 		<!--!-->
- 		<th scope="col" class="myClass" style="font-family:arial black;font-size:X-Large;font-weight:bold;font-style:italic;text-decoration:underline overline line-through;">
+ 		<table style="border-collapse:collapse;"><tbody><tr><td>Simple Widgets</td></tr><tr><!--!--><td style="background-color:Wheat;">First Widget - $7.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Second Widget - $13.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Third Widget - $100.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Fourth Widget - $10.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Fifth Widget - $5.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Sixth Widget - $6.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Seventh Widget - $12.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Eighth Widget - $8.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Ninth Widget - $2.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Tenth Widget - $3.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Eleventh Widget - $16.99</td></tr><tr><!--!--><td style="background-color:Wheat;">Fritz's Widget - $52.70</td></tr><tr><td>End of Line</td></tr></tbody></table>
-                 My Widget List
-             </th>
- 	</tr><tr>
- 		<td style="background-color:Yellow;white-space:nowrap;">
-                 First Widget
-                 <br>
-                 $7.99
-             </td>
- 	</tr><tr>
- 		<td style="color:PapayaWhip;background-color:Black;">Hi!  I'm a separator!  I keep things apart</td>
- 	</tr><tr>
- 		<td style="background-color:Wheat;white-space:nowrap;">
-                 Second Widget
-                 <br>
-                 $13.99
-             </td>
- 	</tr><tr>
- 		<td style="color:PapayaWhip;background-color:Black;">Hi!  I'm a separator!  I keep things apart</td>
- 	</tr><tr>
- 		<td style="background-color:Yellow;white-space:nowrap;">
-                 Third Widget
-                 <br>
-                 $100.99
-             </td>
- 	</tr><tr>
- 		<td style="color:PapayaWhip;background-color:Black;">Hi!  I'm a separator!  I keep things apart</td>
- 	</tr><tr>
- 		<td style="background-color:Wheat;white-space:nowrap;">
-                 Fourth Widget
-                 <br>
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
- <select name="ctl00$MainContent$ddlStatic" id="MainContent_ddlStatic">
+ <!--!--><!--!--><select><option value="" selected="">Select...</option><option value="1">Option One</option><option value="2">Option Two</option><option value="3">Option Three</option></select>
- 	<option value="">Select...</option>
- 	<option value="1">Option One</option>
- 	<option value="2">Option Two</option>
- 	<option value="3">Option Three</option>
- 
- </select>
```

#### DropDownList-2 Diff
```diff
- <select name="ctl00$MainContent$ddlSelected" id="MainContent_ddlSelected">
+ <!--!--><!--!--><select><option value="apple">Apple</option><option value="banana" selected="">Banana</option><option value="cherry">Cherry</option></select>
- 	<option value="apple">Apple</option>
- 	<option selected="selected" value="banana">Banana</option>
- 	<option value="cherry">Cherry</option>
- 
- </select>
```

#### DropDownList-3 Diff
```diff
- <select name="ctl00$MainContent$ddlDataBound" id="MainContent_ddlDataBound">
+ <!--!--><!--!--><select><option value="1">Widget</option><option value="2">Gadget</option><option value="3">Gizmo</option><option value="4">Doohickey</option></select>
- 	<option value="1">First Item</option>
- 	<option value="2">Second Item</option>
- 	<option value="3">Third Item</option>
- 
- </select>
```

#### DropDownList-4 Diff
```diff
- <select name="ctl00$MainContent$ddlDisabled" id="MainContent_ddlDisabled" disabled="disabled" class="aspNetDisabled">
+ <!--!--><!--!--><select><option value="1">9.99</option><option value="2">24.50</option><option value="3">149.95</option><option value="4">3.00</option></select>
- 	<option selected="selected" value="1">Cannot change</option>
- 
- </select>
```

#### DropDownList-5 Diff
```diff
- <select name="ctl00$MainContent$ddlStyled" id="MainContent_ddlStyled" class="form-select">
+ <!--!--><!--!--><select><option value="1">Item: Widget</option><option value="2">Item: Gadget</option><option value="3">Item: Gizmo</option><option value="4">Item: Doohickey</option></select>
- 	<option value="1">Styled</option>
- 
- </select>
```

#### DropDownList-6 Diff
```diff
- <select name="ctl00$MainContent$ddlColors" id="MainContent_ddlColors" style="color:Navy;background-color:LightYellow;width:200px;">
+ <!--!--><!--!--><select disabled=""><option value="">Select...</option><option value="1">Option One</option><option value="2" selected="">Option Two</option><option value="3">Option Three</option></select>
- 	<option value="1">Colored dropdown</option>
- 
- </select>
```

### FileUpload
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| FileUpload | ⚠️ Divergent | Tag structure differs |

#### FileUpload Diff
```diff
- <input type="file" name="ctl00$MainContent$FileUpload1" id="MainContent_FileUpload1" style="">
+ <!--!--><!--!--><!--!--><input type="file" _bl_08635110-ca77-4ed5-a208-df17ebd32cb1="" style="">
-     <input type="submit" name="ctl00$MainContent$btnUpload" value="Upload" id="MainContent_btnUpload" style="">
```

### FormView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| FormView | ❌ Missing in source B | File only exists in first directory |

### GridView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| GridView | ⚠️ Divergent | 33 line differences |

#### GridView Diff
```diff
- <div>
+ <!--!--><!--!--><!--!--><!--!-->
- 	<table cellspacing="0" rules="all" border="1" id="MainContent_CustomersGridView" style="border-collapse:collapse;">
+ 	<!--!-->
- 		<tbody><tr>
+ 	<!--!-->
- 			<th scope="col">CustomerID</th><th scope="col">CompanyName</th><th scope="col">FirstName</th><th scope="col">LastName</th><th scope="col">&nbsp;</th><th scope="col">&nbsp;</th>
+ 	<!--!-->
- 		</tr><tr>
+ 	<!--!-->
- 			<td>1</td><td>Virus</td><td>John</td><td>Smith</td><td>
+ 	<!--!-->
- 				        <button type="button">Click Me! John</button>
+ 	<!--!-->
- 			         </td><td><a href="https://www.bing.com/search?q=Virus John Smith">Search for Virus</a></td>
+ 	<!--!-->
- 		</tr><tr>
+ 	<table class="table table-striped"><thead><tr><th>ID</th><th>CompanyName</th><th>FirstName</th><th>LastName</th><th></th><th></th></tr></thead><tbody><!--!--><!--!--><!--!--><tr><td>1</td><td>Virus</td><td>John</td><td>Smith</td><td><button type="button">Click Me! John</button></td><td><!--!--><!--!--><button type="submit"></button></td></tr><!--!--><!--!--><!--!--><tr><td>2</td><td>Boring</td><td>Jose</td><td>Rodriguez</td><td><button type="button">Click Me! Jose</button></td><td><!--!--><!--!--><button type="submit"></button></td></tr><!--!--><!--!--><!--!--><tr><td>3</td><td>Fun Machines</td><td>Jason</td><td>Ramirez</td><td><button type="button">Click Me! Jason</button></td><td><!--!--><!--!--><button type="submit"></button></td></tr></tbody></table><!--!--><!--!--><!--!--><!--!-->
- 			<td>2</td><td>Boring</td><td>Jose</td><td>Rodriguez</td><td>
+ 		<!--!--><!--!--><!--!-->
- 				        <button type="button">Click Me! Jose</button>
+ 		<!--!--><!--!--><!--!-->
- 			         </td><td><a href="https://www.bing.com/search?q=Boring Jose Rodriguez">Search for Boring</a></td>
+ 		<!--!--><!--!--><!--!-->
- 		</tr><tr>
+ 		<!--!--><!--!--><!--!-->
- 			<td>3</td><td>Fun Machines</td><td>Jason</td><td>Ramirez</td><td>
+ 		<!--!--><!--!-->
- 				        <button type="button">Click Me! Jason</button>
- 			         </td><td><a href="https://www.bing.com/search?q=Fun Machines Jason Ramirez">Search for Fun Machines</a></td>
- 		</tr>
- 	</tbody></table>
- </div>
```

### HiddenField
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| HiddenField | ⚠️ Divergent | Tag structure differs |

#### HiddenField Diff
```diff
- <input type="hidden" name="ctl00$MainContent$HiddenField1" id="MainContent_HiddenField1" value="secret-value-123" style="">
+ <!--!--><!--!--><input id="myHiddenField" type="hidden" value="initial-secret-value" style="">
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
- <a id="MainContent_styleLink" href="https://bing.com" style="color:White;background-color:Blue;">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a id="MainContent_HyperLink1" title="Navigate to Bing!" href="https://bing.com">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a id="MainContent_HyperLink3" href="https://bing.com">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)" style="background-color:DimGray;color:White;">GitHub</a>
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
- <a id="MainContent_styleLink" href="https://bing.com" style="color:White;background-color:Blue;">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="">GitHub</a>
```

#### HyperLink-2 Diff
```diff
- <a id="MainContent_HyperLink1" title="Navigate to Bing!" href="https://bing.com">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank">GitHub</a>
```

#### HyperLink-3 Diff
```diff
- 
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)">GitHub</a>
```

#### HyperLink-4 Diff
```diff
- <a id="MainContent_HyperLink3" href="https://bing.com">Blue Button</a>
+ <!--!--><!--!--><a href="https://www.github.com" target="_blank" title="GitHub (Social Coding)" style="background-color:DimGray;color:White;">GitHub</a>
```

### Image
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Image-1 | ⚠️ Divergent | Tag structure differs |
| Image-2 | ⚠️ Divergent | Tag structure differs |

#### Image-1 Diff
```diff
- <img id="MainContent_imgBasic" src="../../Content/Images/banner.png" alt="Banner image">
+ <!--!--><!--!--><img src="/img/placeholder-150x100.svg" alt="Sample placeholder image" longdesc="">
```

#### Image-2 Diff
```diff
- <img id="MainContent_imgSized" src="../../Content/Images/banner.png" alt="Sized image" style="height:100px;width:200px;">
+ <!--!--><!--!--><img src="/img/placeholder-150x100.svg" alt="Image with tooltip" title="This is a tooltip displayed on hover" longdesc="">
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
- <img id="MainContent_ImageMap1" src="../../Content/Images/banner.png" alt="Navigate" usemap="#ImageMapMainContent_ImageMap1"><map name="ImageMapMainContent_ImageMap1" id="ImageMapMainContent_ImageMap1">
+ <!--!--><!--!--><img src="/img/placeholder-400x200.svg" usemap="#ImageMap_e49cae77d71d4a99bc2acac69c35ef5c" alt="Navigation demo image"><map name="ImageMap_e49cae77d71d4a99bc2acac69c35ef5c"><area shape="rect" coords="0,0,130,200" href="/ControlSamples/Button" alt="Go to Button samples"><area shape="circle" coords="200,100,60" href="/ControlSamples/CheckBox" alt="Go to CheckBox samples"><area shape="rect" coords="270,0,400,200" href="/ControlSamples/Image" alt="Go to Image samples"></map>
- 	<area shape="rect" coords="0,0,100,50" href="https://bing.com" title="Go to Bing" alt="Go to Bing"><area shape="rect" coords="100,0,200,50" href="https://github.com" title="Go to GitHub" alt="Go to GitHub">
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
+ <!--!--><!--!--><span>Hello, World!</span>
```

#### Label-2 Diff
```diff
- <span id="MainContent_lblStyled" class="text-primary" style="color:Blue;font-weight:bold;">Styled Label</span>
+ <!--!--><!--!--><span class="text-danger fw-bold" style="color:Red;">Important notice</span>
```

#### Label-3 Diff
```diff
- <span id="MainContent_lblHtml"><em>Emphasized</em></span>
+ <!--!--><!--!--><label for="emailInput" class="form-label">Email Address:</label>
```

### LinkButton
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| LinkButton-1 | ⚠️ Divergent | Tag structure differs |
| LinkButton-2 | ⚠️ Divergent | Tag structure differs |
| LinkButton-3 | ⚠️ Divergent | Tag structure differs |

#### LinkButton-1 Diff
```diff
- <a id="MainContent_LinkButton1" class="btn btn-primary" href="javascript:__doPostBack('ctl00$MainContent$LinkButton1','')">Click Me</a>
+ <!--!--><!--!--><a>LinkButton1 with Command</a>
```

#### LinkButton-2 Diff
```diff
- <a id="MainContent_LinkButton2" href="javascript:__doPostBack('ctl00$MainContent$LinkButton2','')">Submit Form</a>
+ <!--!--><!--!--><a>LinkButton2 with Command</a>
```

#### LinkButton-3 Diff
```diff
- <a id="MainContent_LinkButton3" class="aspNetDisabled">Disabled Link</a>
+ <!--!--><!--!--><a href="ControlSamples/LinkButton/#">Click me!!</a>
```

### ListBox
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ListBox-1 | ❌ Missing in source B | File only exists in first directory |
| ListBox-2 | ❌ Missing in source B | File only exists in first directory |

### ListView
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ListView | ⚠️ Divergent | 182 line differences |

#### ListView Diff
```diff
- <table>
+ <table class="table"><!--!--><thead><tr><td>Id</td>
-                 <thead>
+ 			<td>Name</td>
-                     <tr>
+ 			<td>Price</td>
-                         <td>Id</td>
+ 			<td>Last Update</td></tr></thead>
-                         <td>Name</td>
+ 	<tbody><!--!--><!--!--><!--!--><tr><td>1</td><!--!-->
-                         <td>Price</td>
+ 					<td>First Widget</td><!--!-->
-                         <td>Last Update</td>
+ 					<td>$7.99</td><!--!-->
-                     </tr>
+ 					<td>2/26/2026</td></tr><!--!--><tr class="table-dark"><td>2</td><!--!-->
-                 </thead>
+ 					<td><!--!--><!--!-->Second Widget</td><!--!-->
-                 <tbody>
+ 					<td><!--!--><!--!-->$13.99</td><!--!-->
-                     
+ 					<td><!--!--><!--!-->2/26/2026</td></tr><!--!--><tr><td>3</td><!--!-->
-             <tr>
+ 					<td>Third Widget</td><!--!-->
-                 <td>1</td>
+ 					<td>$100.99</td><!--!-->
-                 <td>First Widget</td>
+ 					<td>2/26/2026</td></tr><!--!--><tr class="table-dark"><td>4</td><!--!-->
-                 <td>$7.99</td>
+ 					<td><!--!--><!--!-->Fourth Widget</td><!--!-->
-                 <td>2/25/2026</td>
+ 					<td><!--!--><!--!-->$10.99</td><!--!-->
-             </tr>
+ 					<td><!--!--><!--!-->2/26/2026</td></tr><!--!--><tr><td>5</td><!--!-->
-         
+ 					<td>Fifth Widget</td><!--!-->
-           <tr>
+ 					<td>$5.99</td><!--!-->
-             <td colspan="4" style="border-bottom: 1px solid black;">&nbsp;</td>
+ 					<td>2/26/2026</td></tr><!--!--><tr class="table-dark"><td>6</td><!--!-->
... (truncated)
```

### Literal
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Literal-1 | ⚠️ Divergent | Tag structure differs |
| Literal-2 | ⚠️ Divergent | Tag structure differs |
| Literal-3 | ❌ Missing in source B | File only exists in first directory |

#### Literal-1 Diff
```diff
- This is <b>literal</b> content.
+ <!--!--><!--!--><!--!--><b>Literal</b>
```

#### Literal-2 Diff
```diff
- This is &lt;b&gt;encoded&lt;/b&gt; content.
+ <!--!--><!--!--><!--!-->&lt;b&gt;Literal&lt;/b&gt;
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
- <div id="MainContent_pnlUserInfo">
+ <!--!--><!--!--><div><!--!--><p>This content is inside a basic Panel.</p>
- 	<fieldset>
+ 	<!--!--><p>A Panel renders as a div element by default.</p></div>
- 		<legend>
- 			User Info
- 		</legend>
-         <span id="MainContent_lblName">Name:</span>
-         <input name="ctl00$MainContent$txtName" type="text" id="MainContent_txtName" style="">
-     
- 	</fieldset>
- </div>
```

#### Panel-2 Diff
```diff
- <div id="MainContent_pnlScrollable" style="height:100px;overflow:auto;">
+ <!--!--><!--!--><fieldset><legend>User Information</legend><!--!-->
- 	
+ 			<!--!--><p>Name: John Doe</p>
-         <p>First paragraph of content inside the scrollable panel.</p>
+ 	<!--!--><p>Email: john@example.com</p></fieldset>
-         <p>Second paragraph of content inside the scrollable panel.</p>
-         <p>Third paragraph of content inside the scrollable panel.</p>
-         <p>Fourth paragraph of content inside the scrollable panel.</p>
-     
- </div>
```

#### Panel-3 Diff
```diff
- <div id="MainContent_pnlForm" onkeypress="javascript:return WebForm_FireDefaultButton(event, 'MainContent_btnSubmit')">
+ <!--!--><!--!--><div style="background-color:LightYellow;color:Navy;border:2px solid Blue;width:300px;"><!--!--><p>This panel has custom colors and border.</p></div>
- 	
-         <input name="ctl00$MainContent$txtInput" type="text" id="MainContent_txtInput" style="">
-         <input type="submit" name="ctl00$MainContent$btnSubmit" value="Submit" id="MainContent_btnSubmit" style="">
-     
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
+ <!--!--><!--!--><!--!--><p>This content is inside a PlaceHolder.</p>
+ 	<!--!--><p>Note: No extra wrapper element is rendered around this content.</p>
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
+ <!--!--><!--!--><table><tr><td><input id="13cecb93c34c4f14b648080611903111_0" type="radio" name="13cecb93c34c4f14b648080611903111" value="red" style=""><!--!-->
- 	<tbody><tr>
+ 			<label for="13cecb93c34c4f14b648080611903111_0">Red</label></td></tr><tr><td><input id="13cecb93c34c4f14b648080611903111_1" type="radio" name="13cecb93c34c4f14b648080611903111" value="green" style=""><!--!-->
- 		<td><input id="MainContent_RadioButtonList1_0" type="radio" name="ctl00$MainContent$RadioButtonList1" value="Excellent" style=""><label for="MainContent_RadioButtonList1_0">Excellent</label></td>
+ 			<label for="13cecb93c34c4f14b648080611903111_1">Green</label></td></tr><tr><td><input id="13cecb93c34c4f14b648080611903111_2" type="radio" name="13cecb93c34c4f14b648080611903111" value="blue" style=""><!--!-->
- 	</tr><tr>
+ 			<label for="13cecb93c34c4f14b648080611903111_2">Blue</label></td></tr></table>
- 		<td><input id="MainContent_RadioButtonList1_1" type="radio" name="ctl00$MainContent$RadioButtonList1" value="Good" checked="checked" style=""><label for="MainContent_RadioButtonList1_1">Good</label></td>
- 	</tr><tr>
- 		<td><input id="MainContent_RadioButtonList1_2" type="radio" name="ctl00$MainContent$RadioButtonList1" value="Average" style=""><label for="MainContent_RadioButtonList1_2">Average</label></td>
- 	</tr><tr>
- 		<td><input id="MainContent_RadioButtonList1_3" type="radio" name="ctl00$MainContent$RadioButtonList1" value="Poor" style=""><label for="MainContent_RadioButtonList1_3">Poor</label></td>
- 	</tr>
- </tbody></table>
```

#### RadioButtonList-2 Diff
```diff
- <table id="MainContent_RadioButtonList2">
+ <!--!--><!--!--><table><tr><td><input id="8caa14a9fbba41e0bf0ce21aad875666_0" type="radio" name="8caa14a9fbba41e0bf0ce21aad875666" value="S" style=""><!--!-->
- 	<tbody><tr>
+ 			<label for="8caa14a9fbba41e0bf0ce21aad875666_0">Small</label></td><td><input id="8caa14a9fbba41e0bf0ce21aad875666_1" type="radio" name="8caa14a9fbba41e0bf0ce21aad875666" value="M" style=""><!--!-->
- 		<td><input id="MainContent_RadioButtonList2_0" type="radio" name="ctl00$MainContent$RadioButtonList2" value="Yes" style=""><label for="MainContent_RadioButtonList2_0">Yes</label></td><td><input id="MainContent_RadioButtonList2_1" type="radio" name="ctl00$MainContent$RadioButtonList2" value="No" style=""><label for="MainContent_RadioButtonList2_1">No</label></td>
+ 			<label for="8caa14a9fbba41e0bf0ce21aad875666_1">Medium</label></td><td><input id="8caa14a9fbba41e0bf0ce21aad875666_2" type="radio" name="8caa14a9fbba41e0bf0ce21aad875666" value="L" style=""><!--!-->
- 	</tr><tr>
+ 			<label for="8caa14a9fbba41e0bf0ce21aad875666_2">Large</label></td><td><input id="8caa14a9fbba41e0bf0ce21aad875666_3" type="radio" name="8caa14a9fbba41e0bf0ce21aad875666" value="XL" style=""><!--!-->
- 		<td><input id="MainContent_RadioButtonList2_2" type="radio" name="ctl00$MainContent$RadioButtonList2" value="Maybe" style=""><label for="MainContent_RadioButtonList2_2">Maybe</label></td><td></td>
+ 			<label for="8caa14a9fbba41e0bf0ce21aad875666_3">X-Large</label></td></tr></table>
- 	</tr>
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
| Repeater | ⚠️ Divergent | 64 line differences |

#### Repeater Diff
```diff
- This is a list of widgets
+ <table><!--!--><!--!--><!--!--><tr><td>1</td><!--!-->
-           
+ 					<td>First Widget</td><!--!-->
-             <li>First Widget</li>
+ 					<td>$7.99</td><!--!-->
-           <hr>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr class="table-dark"><td>2</td><!--!-->
-             <li>Second Widget</li>
+ 					<td>Second Widget</td><!--!-->
-           <hr>
+ 					<td>$13.99</td><!--!-->
-             <li>Third Widget</li>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr><td>3</td><!--!-->
-           <hr>
+ 					<td>Third Widget</td><!--!-->
-             <li>Fourth Widget</li>
+ 					<td>$100.99</td><!--!-->
-           <hr>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr class="table-dark"><td>4</td><!--!-->
-             <li>Fifth Widget</li>
+ 					<td>Fourth Widget</td><!--!-->
-           <hr>
+ 					<td>$10.99</td><!--!-->
-             <li>Sixth Widget</li>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr><td>5</td><!--!-->
-           <hr>
+ 					<td>Fifth Widget</td><!--!-->
-             <li>Seventh Widget</li>
+ 					<td>$5.99</td><!--!-->
-           <hr>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr class="table-dark"><td>6</td><!--!-->
-             <li>Eighth Widget</li>
+ 					<td>Sixth Widget</td><!--!-->
-           <hr>
+ 					<td>$6.99</td><!--!-->
-             <li>Ninth Widget</li>
+ 					<td>2/26/2026</td></tr><!--!--><tr><td colspan="4"><hr></td></tr><!--!--><tr><td>7</td><!--!-->
-           <hr>
+ 					<td>Seventh Widget</td><!--!-->
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
| SiteMapPath-1 | ❌ Missing in source B | File only exists in first directory |
| SiteMapPath-2 | ❌ Missing in source B | File only exists in first directory |

### Table
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| Table-1 | ❌ Missing in source B | File only exists in first directory |
| Table-2 | ❌ Missing in source B | File only exists in first directory |
| Table-3 | ❌ Missing in source B | File only exists in first directory |

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
| TreeView | ⚠️ Divergent | 37 line differences |

#### TreeView Diff
```diff
- <a href="#MainContent_SampleTreeView_SkipLink" style="position:absolute;left:-10000px;top:auto;width:1px;height:1px;overflow:hidden;">Skip Navigation Links.</a><div id="MainContent_SampleTreeView" class="Foo">
+ <!--!--><!--!--><div id="SampleTreeView"><!--!--><!--!-->
- 	<table cellpadding="0" cellspacing="0" style="border-width:0;">
+ <!--!-->
- 		<tbody><tr>
+ <!--!-->
- 			<td><a href="Home.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn0');" id="MainContent_SampleTreeViewn0i" tabindex="-1"><img src="../../Content/Images/C#.png" alt="This is the home image tooltip" title="This is the home image tooltip" style="border-width:0;"></a></td><td style="white-space:nowrap;"><input type="checkbox" name="MainContent_SampleTreeViewn0CheckBox" id="MainContent_SampleTreeViewn0CheckBox" checked="checked" style=""><a class="MainContent_SampleTreeView_0" href="Home.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn0');" id="MainContent_SampleTreeViewn0">Home</a></td>
+ <!--!-->
- 		</tr>
+ <!--!-->
- 	</tbody></table><div id="MainContent_SampleTreeViewn0Nodes" style="display:block;">
+ <!--!-->
- 		<table cellpadding="0" cellspacing="0" style="border-width:0;">
+ <!--!--><!--!--><!--!--><table cellpadding="0" cellspacing="0" style="border-width: 0;"><tr><!--!--> <td><a href=""><img src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" alt="Collapse Home" title="Collapse Home" style="border-width: 0;"></a></td><td style="white-space: nowrap; "><input type="checkbox" style=""><a href="/" target="Content" id="MainContent_SampleTreeViewt0">Home</a></td></tr></table><!--!-->
- 			<tbody><tr>
+ <!--!--><!--!--><table cellpadding="0" cellspacing="0" style="border-width: 0;"><tr><!--!--><td><div style="width:20px;height:1px;"></div></td><td><img src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" alt=""></td><td style="white-space: nowrap; "><a id="MainContent_SampleTreeViewt0">Foo</a></td></tr></table><!--!-->
- 				<td><div style="width:20px;height:1px"></div></td><td style="white-space:nowrap;"><input type="checkbox" name="MainContent_SampleTreeViewn1CheckBox" id="MainContent_SampleTreeViewn1CheckBox" title="ToolTop" style=""><a class="MainContent_SampleTreeView_0" href="Page1.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn1');" title="ToolTop" id="MainContent_SampleTreeViewn1">Page1</a></td>
+ <!--!--><!--!-->
- 			</tr>
+ 			<!--!--><table cellpadding="0" cellspacing="0" style="border-width: 0;"><tr><!--!--><td><div style="width:20px;height:1px;"></div></td><td><a href=""><img src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" alt="Collapse Bar" title="Collapse Bar" style="border-width: 0;"></a></td><td style="white-space: nowrap; "><input type="checkbox" style=""><a id="MainContent_SampleTreeViewt0">Bar</a></td></tr></table><!--!-->
- 		</tbody></table><div id="MainContent_SampleTreeViewn1Nodes" style="display:none;">
+ <!--!--><!--!--><table cellpadding="0" cellspacing="0" style="border-width: 0;"><tr><!--!--><td><div style="width:20px;height:1px;"></div></td><td><div style="width:20px;height:1px;"></div></td><td><a href=""><img src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_Collapse.gif" alt="Collapse Baz" title="Collapse Baz" style="border-width: 0;"></a></td><td style="white-space: nowrap; "><input type="checkbox" style=""><a id="MainContent_SampleTreeViewt0">Baz</a></td></tr></table><!--!-->
- 			<table cellpadding="0" cellspacing="0" style="border-width:0;">
+ <!--!--><!--!--><table cellpadding="0" cellspacing="0" style="border-width: 0;"><tr><!--!--><td><div style="width:20px;height:1px;"></div></td><td><div style="width:20px;height:1px;"></div></td><td><div style="width:20px;height:1px;"></div></td><td><img src="_content/Fritz.BlazorWebFormsComponents/TreeView/Default_NoExpand.gif" alt=""></td><td style="white-space: nowrap; "><a id="MainContent_SampleTreeViewt0">BlazorMisterMagoo</a></td></tr></table><!--!-->
- 				<tbody><tr>
+ <!--!--><!--!-->
- 					<td><div style="width:20px;height:1px"></div></td><td><div style="width:20px;height:1px"></div></td><td style="white-space:nowrap;"><input type="checkbox" name="MainContent_SampleTreeViewn2CheckBox" id="MainContent_SampleTreeViewn2CheckBox" style=""><a class="MainContent_SampleTreeView_0" href="Section1.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn2');" id="MainContent_SampleTreeViewn2">Section 1</a></td>
+ </div>
- 				</tr>
- 			</tbody></table>
- 		</div><table cellpadding="0" cellspacing="0" style="border-width:0;">
- 			<tbody><tr>
- 				<td><div style="width:20px;height:1px"></div></td><td style="white-space:nowrap;"><input type="checkbox" name="MainContent_SampleTreeViewn3CheckBox" id="MainContent_SampleTreeViewn3CheckBox" style=""><a class="MainContent_SampleTreeView_0" href="Page2.aspx" target="Content" onclick="javascript:TreeView_SelectNode(MainContent_SampleTreeView_Data, this,'MainContent_SampleTreeViewn3');" id="MainContent_SampleTreeViewn3">Page 2</a></td>
- 			</tr>
- 		</tbody></table>
- 	</div>
- </div><a id="MainContent_SampleTreeView_SkipLink"></a>
```

### ValidationSummary
| Variant | Status | Diff Summary |
|---------|--------|-------------|
| ValidationSummary-1 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-2 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-3 | ❌ Missing in source B | File only exists in first directory |
| ValidationSummary-Submit | ❌ Missing in source B | File only exists in first directory |
