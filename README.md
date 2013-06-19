Stars
=====

A demo of the F# Sql Server type provider: build a simple planetarium.

Content
=======

The demo displays a simple planetarium.  Use arrow keys to move the direction of view.

Limitations/Bugs
================

*** Nothing is displayed until the first time you hit an arrow key. ***

Only displays stars, not planets etc.

The demo assumes a rectangular viewport, so star locations near the pole will be increasingly distorted.

The demo has no concept of wraparound, so panning will eventually take you to an empty void.

This is demo is NOT architected correctly: it's a demo of the F# type provider, nothing more.

Credits
=======

The original data come from this magnificent repo:

https://github.com/astronexus/HYG-Database

The mapping from stellar spectrum to RGB value comes from here:

http://www.vendian.org/mncharity/dir3/starcolor/

Disclaimer
==========

This is a demo.  Do not use for any purpose other than understanding F#.

Setup
=====

This demo needs a SQL Server database. To set up:

Either: 

- Restore the SQL Server 2012 backup stars.bak

Or:

- Unzip and open the file databasesetup.sql and change values in the first few lines to match your SQL Server setup.
- Run it using SQLCmd - eg. sqlcmd -S myserver -i databasesetup.sql or using Sql Server Management Studio

Then:

- If necessary, edit the connection string in StarsDatabase.fs to reference the location where the database was set up.
