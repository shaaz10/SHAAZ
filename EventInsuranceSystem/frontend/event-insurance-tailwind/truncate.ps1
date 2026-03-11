$path = "c:\Users\Lenovo\Desktop\Capstone\EventInsuranceSystem\event-insurance-tailwind\src\app\features\customer-dashboard\customer-dashboard.component.html"
$content = Get-Content $path
$trimmed = $content[0..534]
$trimmed | Set-Content $path -Encoding UTF8
Write-Host "Done. Kept $($trimmed.Count) lines."
