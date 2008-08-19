# Gurtle
# Copyright (c) 2008 Atif Aziz. All rights reserved.
#
#  Author(s):
#
#      Atif Aziz, http://www.raboof.com
#
# This library is free software; you can redistribute it and/or modify it 
# under the terms of the New BSD License, a copy of which should have 
# been delivered along with this distribution.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
# "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
# LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
# PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
# OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
# LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
# DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
# THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
# OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

import sys, re

from System import Uri
from System.IO import Path, File, FileNotFoundException
from System.Diagnostics import Process

def main(args):
    install_path = Path.GetDirectoryName(Path.GetFullPath(sys.argv[0]))
    build = args and args.pop(0) or 'Release'    
    dll_path = Path.Combine(install_path, r'bin\%s\Gurtle.dll' % build)
    if not File.Exists(dll_path):
        raise FileNotFoundException('The file "%s" does not exist. Did you forget to compile?' % dll_path)
    reg = File.ReadAllText(Path.Combine(install_path, 'install.reg'))
    reg = re.compile('^"CodeBase"=".+?"', re.M).sub('"CodeBase"="%s"' % Uri(dll_path), reg, 2)    
    reg_path = Path.Combine(Path.GetTempPath(), 'gurtle.reg')
    File.WriteAllText(reg_path, reg)
    try:
        Process.Start(reg_path).Dispose()
    finally:
        File.Delete(reg_path)

if __name__ == '__main__':
    try:
        main(sys.argv[1:])
    except Exception, e:
        print >> sys.stderr, e
