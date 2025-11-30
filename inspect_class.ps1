$asm = [System.Reflection.Assembly]::LoadFrom("C:\git\js2il\out_test_destr\test_destr_simple.dll")
$pointType = $asm.GetType("Classes.Point")
Write-Host "Fields:"
$pointType.GetFields([System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance) | ForEach-Object { Write-Host "  $($_.Name) : $($_.FieldType)" }
Write-Host "`nConstructors:"
$pointType.GetConstructors() | ForEach-Object {
    Write-Host "  .ctor($([string]::Join(', ', ($_.GetParameters() | ForEach-Object { "$($_.ParameterType) $($_.Name)" }))))"
}
