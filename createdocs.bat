@echo off

lib\mdoc\mdoc update -o doc\en -i build_output\Magnum\Magnum.xml build_output\Magnum\Magnum.dll --exceptions=added 
lib\mdoc\mdoc export-html -o doc\html doc\en

