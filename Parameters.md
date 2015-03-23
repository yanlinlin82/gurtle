# Parameters #

(These instructions mainly apply to ToroiseSVN. For TortoiseGit, see [3.34. Integration with Bug Tracking Systems / Issue Trackers](http://tortoisegit.org/docs/tortoisegit/tsvn-dug-bugtracker.html))

Once you have Gurtle installed on your system, there are two ways to enable it in TortoiseSVN (TSVN) for a given Google Code project. If you are using TSVN 1.5 then the parameters have to be configured manually in TSVN Settings for each working copy  and this has to be done by each member of the project on his or her workstation(s). If you are using TSVN version 1.6 or later then you can set the Gurtle and TSVN configuration in properties in the project's Subversion repository once. Each project member will then simply inherit the settings by checking out or updating their working copy and Gurtle will be automatically enabled for the project in TSVN.

The parameters for Gurtle is a semi-colon delimited list of name and value pairs where the name and value within each pair is separated by an equal (=) sign. The syntax can be summarized as follows:

```
PARAMETERS = PARAMETER ( ";" PARAMETER ) *
PARAMETER  = NAME "=" VALUE
```

The possible values for NAME and VALUE are described in the table below:

| NAME | VALUE |
|:-----|:------|
| `project` | Name (always in lowercase) of the project hosted at Google Code. For the example, for the project at http://gurtle.googlecode.com/, the value for this parameter would be `gurtle`.  |

## Local Configuration via TSVN Settings ##

This section applies to TSVN 1.5 or later.

You configure a working copy path for Gurtle in the Issue Tracker Integration area of TSVN Settings. You will need to supply values for the **Parameters** field:

![http://gurtle.googlecode.com/svn/wiki/SettingsIssueTracker.png](http://gurtle.googlecode.com/svn/wiki/SettingsIssueTracker.png)

If you are using TSVN 1.6 or later, you will see an **Options** button (as shown above) that you can use to configure the parameters interactively. Pressing the button will display the following dialog box using which, for example, you can specify the Google Code project:

![http://gurtle.googlecode.com/svn/wiki/OptionsDialog.png](http://gurtle.googlecode.com/svn/wiki/OptionsDialog.png)

## Central Configuration via SVN Repository ##

This section applies to TSVN 1.6 or later.

TSVN 1.6 or later can pick up the parameters automatically from a working copy of a project by looking up the `bugtraq:provideruuid`, `bugtraq:provideruuid64` and `bugtraq:providerparams` properties. This enables  all users on a project to be synchronized on the same issue tracker plug-in and settings.

For Gurtle, `bugtraq:provideruuid` and `bugtraq:provideruuid64` must always be set to `{91974081-2DC7-4FB1-B3BE-0DE1C8D6CE4E}` (32 bit installations) and `{A0557FA7-7C95-485b-8F40-31303F762C57}` (64-bit installations), respectively. The `bugtraq:providerparams` property should be set to Gurtle-specific parameters described above, e.g. `project=gurtle`.

Below is an example of how you can set these properties from the command-line (where `ZZZ` should be replaced with the Google Code project name _in lowercase_):

```
$ svn propset bugtraq:provideruuid {91974081-2DC7-4FB1-B3BE-0DE1C8D6CE4E} .
property 'bugtraq:provideruuid' set on '.'

$ svn propset bugtraq:provideruuid64 {A0557FA7-7C95-485b-8F40-31303F762C57} .
property 'bugtraq:provideruuid' set on '.'

$ svn propset bugtraq:providerparams project=ZZZ .
property 'bugtraq:providerparams' set on '.'

$ svn commit -m "Added TSVN/Gurtle IBugTraqProvider properties."
```
