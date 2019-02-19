# FiddlerPasswordRemoval
This windows forms based app helps to remove passwords captured while taking a fiddler trace.
This particulary useful to safeguard user account information while passing the fiddler trace to other users.

Source code is available as a zip file. Feel free to download and make changes.

ReadFile() method uses ZipFile class to open the .saz file and ZipArchiveEntry class to iterate through all fiddler entries to check if passwd field is found. If it is found then it replaces it with "DELETED".
