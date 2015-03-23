Starting with version 0.5, Gurtle can automatically close issues with a reference to the revision number when you make a commit via TortoiseSVN (1.6 or later) and TortoiseGit. Since [Google Code does not have an API to its issue tracker yet](http://code.google.com/p/support/issues/detail?id=148), Gurtle uses a backdoor means to update issues by doing an HTML form submission, much the same way a person would do it through the project web site. The downside of this approach is, however, that Google Code can change the form processing and break Gurtle. To avoid having everyone update their Gurtle installation each time this happens, the form submission has not been hard-wired into Gurtle. Instead, Gurtle expects an external script to carry out this task and which can be updated independently and even maintained by the community.

To enable the issue update UI in Gurtle, start by downloading the latest version of the script by exporting http://gurtle.googlecode.com/svn/scripts/ using a Subversion client like TortoiseSVN.

Next, define an environment variable named `GURTLE_ISSUE_UPDATE_CMD` and set its value to :

```
updissue.cmd --username {username} --password {password} --project {project} --issue {issue.id} --status {status} --comment {comment} --debug
```

Prepend `updissue.cmd` with the path where you exported the scripts earlier.

That's it! Next time you make a commit via TortoiseSVN/TortoiseGit and use Gurtle to select one or more issues, you will see a dialog box like the following show up _after_ the commit and using which you will be able to set the comment and status for each issue.

If you start finding that you are getting errors during issue updates, make sure to check back and see if there is a new version of the script available. You are also encouraged to leave the `--debug` flag that could aid in identifying and [reporting issues](http://code.google.com/p/gurtle/issues/entry).

![http://gurtle.googlecode.com/svn/wiki/IssueUpdateDialog.png](http://gurtle.googlecode.com/svn/wiki/IssueUpdateDialog.png)