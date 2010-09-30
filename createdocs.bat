@echo off

lib\mdoc\mdoc update -o doc\en -i build_output\Stact\Stact.xml build_output\Stact\Stact.dll --exceptions=added 
lib\mdoc\mdoc export-html -o doc\html doc\en

