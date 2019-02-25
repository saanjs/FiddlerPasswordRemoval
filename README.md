# FiddlerPasswordRemoval
This windows forms based app helps to remove passwords captured while taking a fiddler trace.
This particulary useful to safeguard user account information while passing the fiddler trace to other users.

Source code is available as a zip file. Feel free to download and make changes.

A fiddler trace (.saz) file is quite similar like a windows archive file like .zip. So, System.IO.Compression can still be hepful in this scenario to read .saz files and manipulate it as per the need.

ReadFile() method uses ZipFile class to open the .saz file and ZipArchiveEntry class to iterate through all fiddler entries to check if passwd field is found. If it is found then it replaces it with "DELETED" and saves the original file.

It checks for ".saz" extensions and also uses ProgressBar control to display the progress.

<b> Update 21-02-2019: It can now handle .har files </b></br>
<b> Update 25-02-2019 : It can now scrub passwords captured through forms based authentication too </b>
