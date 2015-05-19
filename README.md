WPF Minitrend Usercontrol
=========================

This project contains an example of how a minitrend can be implemented in WPF.

Getting Started
---------------
The Usercontrol stores a copy of the data in an array with a fixed size of 180. 
Data is updated through the Update method of the user control. The new value is
passed as a parameter. An internal counter keeps track of last index.

Most of the configuration can be handled when you declare the user control in
the xaml. Most importantly, the Ymin and Ymax properties set the y axis range.
The Units parameter specifies the text that is placed to the right of the 
current value.

The Timebase parameter specifies (and only specifies) the text below the graph 
to the left. To actually control the timebase you simply control the frequency
that you update the minitrend.

Every time update is called, the plot shifts by 1 pixel. The graph portion of the 
trend is 180 pixels wide, so if you update it once a second it would have a 
timebase of 180 seconds or 3 minutes. So if you want a time base of 5 minutes 
your interval should be 3.333 (600.0 / 180.0).

It is not the most sophisticated thing in the world, but hopefully it will get 
you started.

