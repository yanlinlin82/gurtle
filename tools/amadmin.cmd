@"%~dp0IronPython\ipy" -x "%~f0" %* & goto :EOF
import sys
from System import Environment
from System.Security.Principal import \
    WindowsIdentity, WindowsPrincipal, WindowsBuiltInRole

def main():
    user = WindowsPrincipal(WindowsIdentity.GetCurrent())
    if user.IsInRole(WindowsBuiltInRole.Administrator):
        print 'You are a system administrator.'
    else:
        print 'You are NOT a system administrator.'
        sys.exit(1)

if __name__ == '__main__':
    main()
