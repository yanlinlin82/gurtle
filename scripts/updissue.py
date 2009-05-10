import sys
from sys import stderr
from System import Uri, DateTime, Convert
from System.IO import File
from System.Collections.Specialized import NameValueCollection
from System.Net import WebClient as SysWebClient, CookieContainer
from System.Globalization import NumberFormatInfo
from System.Text.RegularExpressions import Regex
from System.Text import Encoding

class WebClient(SysWebClient):
    def __init__(self):
        self.Method = None
        self.CookieContainer = None
    def GetWebRequest(self, address):
        req = SysWebClient.GetWebRequest(self, address)
        if self.Method:
            req.Method = self.Method
        if self.CookieContainer:
            req.CookieContainer = self.CookieContainer
        return req

def format_form_data(form, boundary):
    fd = []
    for i in range(form.Count):
        name = form.GetKey(i)
        for value in form.GetValues(i):
            fd.append('--' + boundary)
            fd.append('Content-Disposition: form-data; name="%s"' % name)
            fd.append('')
            fd.append(value)
    fd.append('--' + boundary + '--')
    return '\r\n'.join(fd)
    
def update_issue(username, password, project, issue, status, comment, dry_run = False):

    wc = WebClient()
    wc.CookieContainer = CookieContainer()

    # Login

    print >> stderr, wc.DownloadString('https://www.google.com/accounts/ServiceLoginAuth?service=mail&Email=%s&Passwd=%s' % (username, password))
    cookies = wc.CookieContainer.GetCookies(Uri('http://code.google.com/'))
    print >> stderr, '\n'.join(['%s=%s' % (cookie.Name, cookie.Value) for cookie in cookies])
    sid = cookies['SID']
    if not sid or not sid.Value:
        raise Exception('Authentication failed.')
        
    # Get editing token

    html = wc.DownloadString('http://code.google.com/p/%s/issues/bulkedit?ids=%d' % (project, issue))
    token = Regex.Match(html, 'name="token" value="([0-9a-f]+)"').Groups[1].Value

    # Set up form/update to post with mandatory data

    form = NameValueCollection()
    form['can'] = '1'
    form['start'] = '0'
    form['num'] = '100'
    form['q'] = ''
    form['sort'] = ''
    form['colspec'] = ''
    form['issue_ids'] = str(issue)
    form['token'] = token
    form['comment'] = comment or 'This is a test comment bulk-posted by a script. ' + DateTime.UtcNow.ToString('r')
    
    # optional...

    if status:
        form['status'] = status

    #form['owner'] = ''
    #form['cc'] = ''
    #form.Add('label', '')
    #form['btn'] = 'Update 1 Issue'
    
    # Bombs away!

    boundary = DateTime.Now.Ticks.ToString(NumberFormatInfo.InvariantInfo).PadLeft(38, '-')
    form_data = format_form_data(form, boundary)
    print >> stderr, form_data
    wc.Headers['Content-Type'] = 'multipart/form-data; boundary=' + boundary
    form_data_bytes = Encoding.UTF8.GetBytes(form_data)
    if not dry_run:
        wc.UploadData('http://code.google.com/p/%s/issues/bulkedit.do' % project, form_data_bytes)
    
def parse_options(args, names, flags = None, lax = False):
    args = list(args) # copy for r/w
    required = [name[:-1] for name in names if '!' == name[-1:]]
    all = [name.rstrip('!') for name in names]
    if flags:
        all.extend(flags)
    options = {}
    anon = []
    while args:
        arg = args.pop(0)
        if arg[:2] == '--':
            name = arg[2:]
            if not name: # comment
                break
            if not name in all:
                if not lax:
                    raise Exception('Unknown argument: %s' % name)
                anon.append(arg)
                continue
            if flags and name in flags:
                options[name] = True
            elif args:
                options[name] = args.pop(0)
            else:
                raise Exception('Missing argument value: %s' % name)
        else:
            anon.append(arg)
    for name in required:
        if not name in options:
            raise Exception('Missing required argument: %s' % name)
    return options, anon
    
def decode_comment(comment):
    # A comment can be in-line or file-based. A file-based comment
    # will always start with @, with the remaining portion being
    # the path to the file containing the comment. However, if the
    # comment starts with two @, then it is still in-line except the
    # comment really starts from the second character. All other 
    # cases are in-line comments.

    if comment and len(comment) > 1 and comment[0] == '@':
        if comment[1] != '@':
            return File.ReadAllText(comment[1:], Encoding.UTF8)
        else:
            return comment[1:]
    return comment

def main(args):

    options, tails = parse_options(args, 
        ('username!', 'password!', 'project!', 'issue!', 'status', 'comment!'),
        ('dry-run', ))
    
    comment = decode_comment(options.get('comment', None))
    password = Encoding.UTF8.GetString(Convert.FromBase64String(options['password']))
    
    update_issue(options['username'], password, options['project'], 
        int(options['issue']), options.get('status', None), comment, 
        options.get('dry-run', False))

if __name__ == '__main__':
    try:
        main(sys.argv[1:])
    except Exception, e:
        print >> sys.stderr, e
        sys.exit(1)
