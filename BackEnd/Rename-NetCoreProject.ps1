# Rename-NetCoreProject.ps1

param(
    [string]$ProjectPath = ".",
    [string]$OldName = "Cashif",
    [string]$NewName = "Garage"
)

# Change to project directory
Set-Location $ProjectPath

Write-Host "Starting project rename from '$OldName' to '$NewName'..." -ForegroundColor Green

# 1. Rename solution file
Get-ChildItem -Filter "*$OldName*.sln" | ForEach-Object {
    $newFileName = $_.Name -replace $OldName, $NewName
    Rename-Item -Path $_.FullName -NewName $newFileName
    Write-Host "Renamed solution: $($_.Name) -> $newFileName" -ForegroundColor Yellow
    
    # Update content of solution file
    (Get-Content $newFileName) -replace $OldName, $NewName | Set-Content $newFileName
}

# 2. Rename project files (.csproj)
Get-ChildItem -Filter "*$OldName*.csproj" -Recurse | ForEach-Object {
    $newFileName = $_.Name -replace $OldName, $NewName
    Rename-Item -Path $_.FullName -NewName $newFileName
    Write-Host "Renamed project: $($_.Name) -> $newFileName" -ForegroundColor Yellow
    
    # Update project references inside csproj
    (Get-Content $newFileName) -replace $OldName, $NewName | Set-Content $newFileName
}

# 3. Rename folders
Get-ChildItem -Directory -Filter "*$OldName*" -Recurse | ForEach-Object {
    $newDirName = $_.Name -replace $OldName, $NewName
    $parentPath = Split-Path -Parent $_.FullName
    $newPath = Join-Path $parentPath $newDirName
    Rename-Item -Path $_.FullName -NewName $newDirName
    Write-Host "Renamed folder: $($_.Name) -> $newDirName" -ForegroundColor Yellow
}

# 4. Update namespace in all .cs files
Get-ChildItem -Filter "*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName
    $newContent = $content -replace "namespace\s+$OldName", "namespace $NewName"
    $newContent = $newContent -replace "using\s+$OldName", "using $NewName"
    $newContent = $newContent -replace "class\s+$OldName", "class $NewName"
    $newContent | Set-Content $_.FullName
    Write-Host "Updated namespace in: $($_.Name)" -ForegroundColor Cyan
}

# 5. Update .cshtml files (for MVC/Razor Pages)
Get-ChildItem -Filter "*.cshtml" -Recurse | ForEach-Object {
    (Get-Content $_.FullName) -replace $OldName, $NewName | Set-Content $_.FullName
    Write-Host "Updated view: $($_.Name)" -ForegroundColor Cyan
}

# 6. Update JSON config files (appsettings.json, launchSettings.json, etc.)
Get-ChildItem -Filter "*.json" -Recurse | ForEach-Object {
    (Get-Content $_.FullName) -replace $OldName, $NewName | Set-Content $_.FullName
    Write-Host "Updated JSON: $($_.Name)" -ForegroundColor Cyan
}

# 7. Update Program.cs and Startup.cs if they exist
$programFiles = @("Program.cs", "Startup.cs")
foreach ($file in $programFiles) {
    Get-ChildItem -Filter $file -Recurse | ForEach-Object {
        (Get-Content $_.FullName) -replace $OldName, $NewName | Set-Content $_.FullName
        Write-Host "Updated: $($_.Name)" -ForegroundColor Cyan
    }
}

Write-Host "Project rename completed successfully!" -ForegroundColor Green
Write-Host "Please rebuild your project and check for any remaining references." -ForegroundColor Yellow