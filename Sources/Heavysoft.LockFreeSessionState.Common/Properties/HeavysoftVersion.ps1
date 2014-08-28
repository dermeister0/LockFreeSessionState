#-------------------------------------------------------------------------------
# Description: Sets the AssemblyVersion and AssemblyFileVersion of 
#              AssemblyInfo.cs files.
# Author: Luis Rocha
# Version: 1.0
#-------------------------------------------------------------------------------

$major    = 1
$minor    = 0
$revision = 0

#-------------------------------------------------------------------------------
# Displays how to use this script.
#-------------------------------------------------------------------------------
function Help {
    "Sets the AssemblyVersion and AssemblyFileVersion of AssemblyInfo.cs files`n"
    ".\SetVersion.ps1 [VersionNumber]`n"
    "   [VersionNumber]     The version number to set, for example: 1.1.9301.0"
    "                       If not provided, a version number will be generated.`n"
}

#-------------------------------------------------------------------------------
# Generate a version number.
# Note: customize this function to generate it using your version schema.
#-------------------------------------------------------------------------------
function Generate-VersionNumber {
    $startDate = Get-Date -Date "2001-10-13 00:00:00Z"
    $today = (Get-Date).ToUniversalTime()
    $build = ($today - $startDate).Days
    
    return "$major.$minor.$build.$revision"
}
 
#-------------------------------------------------------------------------------
# Update version numbers of AssemblyInfo.cs
#-------------------------------------------------------------------------------
function Update-AssemblyInfoFiles ([string] $version) {
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $version + '")';
    $fileVersion = 'AssemblyFileVersion("' + $version + '")';
    
    Get-ChildItem -r -filter HeavysoftVersion.cs.template | ForEach-Object {
        $templateName = $_.Directory.ToString() + '\' + $_.Name
        $outputName   = $_.Directory.ToString() + '\HeavysoftVersion.cs'
        $outputName + ' -> ' + $version
        
        # If you are using a source control that requires to check-out files before 
        # modifying them, make sure to check-out the file here.
        # For example, TFS will require the following command:
        # tf checkout $filename
    
        (Get-Content $templateName) | ForEach-Object {
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $outputName
    }
}

#-------------------------------------------------------------------------------
# Parse arguments.
#-------------------------------------------------------------------------------
if ($args -ne $null) {
    $version = $args[0]
    if (($version -eq '/?') -or ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}")) {
        Help
        return;
    }
} else {
    $version =  Generate-VersionNumber
}

Update-AssemblyInfoFiles $version
