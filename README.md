# Task Switchy

A friend remarked that he wanted to use the IntelliJ/Resharper style of switching files/workspaces in Windows. This was built as a proof-of-concept, but due to my friends familiarity with Java, we later changedinto that and developed it further there.

The Java project is significantly more full-featured now, but includes proprietary links into company systems as it acts a more generic plugin launcher for Windows shortcuts now, of which task switching is only one.

So, this is just a toy project taking a couple of hours and it demonstrates a few interactions with Win32 from .NET to accomplish a hidden exe that you can launch by hitting Alt-'  (that's ALT grave accent/keycode 192).

It will find your window as best as it can using camel case matching of window names & it will show a little preview of the window you want to switch to. 
