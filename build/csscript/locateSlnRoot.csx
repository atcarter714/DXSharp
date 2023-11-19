#load ".\include\DirectoryHelper.csx"
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// ----------------------------------------------------
// Example of how to use "DirectoryHelper.csx" in CSX:
// Load include files as shown above and below:
// #load ".\include\FileName.csx"
// Build scripts and tools can easily find just about
// anything they may need to read or operate on ...
// ----------------------------------------------------

Console.WriteLine( $"Root solution folder is:\n" ) ;
Console.WriteLine( DirectoryHelper.RootSolutionDir ) ;

Console.WriteLine( "-----------------------------" ) ;
Console.WriteLine( $"Solution Name    : {DirectoryHelper.SolutionName}" ) ;
Console.WriteLine( $"Output Bin Dir   : {DirectoryHelper.RootSolutionBinDir}" ) ;
Console.WriteLine( $"Tests Output Dir : {DirectoryHelper.TestProjectMainOutputDir}" ) ;
Console.WriteLine( $"Source Folder    : {Path.Combine( DirectoryHelper.RootSolutionDir, DirectoryHelper.SourceFolderName )}" ) ;
Console.WriteLine( $"Samples Folder   : {Path.Combine( DirectoryHelper.RootSolutionDir, DirectoryHelper.SamplesFolderName )}" ) ;
Console.WriteLine( "-----------------------------" ) ;

// ----------------------------------------------------