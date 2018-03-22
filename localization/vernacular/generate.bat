ECHO OFF

REM Android
ECHO Generating Android resource files...

if not exist "..\..\src\Android\Resources\values" mkdir "..\..\src\Android\Resources\values"
Vernacular.exe --input="..\strings\dump-en_US.po" --output="..\..\src\Android\Resources\values\strings.xml" --generator=android

if not exist "..\..\src\Android\Resources\values-it" mkdir "..\..\src\Android\Resources\values-it"
Vernacular.exe --input="..\strings\dump-it_IT.po" --output="..\..\src\Android\Resources\values-it\strings.xml" --generator=android

if not exist "..\..\src\Android\Resources\values-it" mkdir "..\..\src\Android\Resources\values-ro"
Vernacular.exe --input="..\strings\dump-ro_RO.po" --output="..\..\src\Android\Resources\values-ro\strings.xml" --generator=android

REM iOS
ECHO Generating iOS resource files...

Vernacular.exe --input="..\strings\dump-en_US.po" --output="..\..\src\iOS\en.lproj\Localizable.strings" --generator=ios

Vernacular.exe --input="..\strings\dump-it_IT.po" --output="..\..\src\iOS\it.lproj\Localizable.strings" --generator=ios

Vernacular.exe --input="..\strings\dump-ro_RO.po" --output="..\..\src\iOS\ro.lproj\Localizable.strings" --generator=ios

ECHO All done.