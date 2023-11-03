echo y| git mergetool --no-prompt
timeout 1
del /s *.orig
del /s *.swp
