$baseUrl = "http://localhost:5194"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

Write-Output "--- Testing Stored Procedure Revenue Report ---"
try {
    $reportRes = Invoke-WebRequest -Uri "$baseUrl/Reports/StoredProcedureRevenueReport" -WebSession $session
    Write-Output "Report Page Title: $([regex]::match($reportRes.Content, '<title>(.*?)</title>').Groups[1].Value)"
    Write-Output "Report Page status: $($reportRes.StatusCode)"
} catch {
    Write-Output "Report Page request failed: $_"
}

Write-Output "--- Testing Order Creation ---"
$getRes = Invoke-WebRequest -Uri "$baseUrl/Order/Create" -WebSession $session
$tokenPattern = 'input name="__RequestVerificationToken" type="hidden" value="([^"]+)"'
if ($getRes.Content -match $tokenPattern) {
    $token = $Matches[1]
    Write-Output "Found token: $token"
    
    $body = @{
        "__RequestVerificationToken" = $token
        "CustomerName" = "Test Customer"
        "InventoryId" = "1"
        "OrderDate" = "2026-06-04"
        "TotalAmount" = "25.99"
    }
    try {
        $postRes = Invoke-WebRequest -Uri "$baseUrl/Order/Create" -Method Post -Body $body -WebSession $session -MaximumRedirection 0
        Write-Output "POST Order/Create status: $($postRes.StatusCode)"
    } catch {
        Write-Output "POST Order/Create failed (expected 302 redirect): $_"
        if ($_.Exception.Response) {
            $stream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($stream)
            $errContent = $reader.ReadToEnd()
            Write-Output "Error content length: $($errContent.Length)"
        }
    }
} else {
    Write-Output "Token not found in GET response"
}

Write-Output "--- Testing Inventory Deletion of Unused Item ---"
# Create a temporary item to delete (so we don't violate order FK constraint)
$getItemRes = Invoke-WebRequest -Uri "$baseUrl/Inventory/Create" -WebSession $session
if ($getItemRes.Content -match $tokenPattern) {
    $token = $Matches[1]
    
    $createBody = @{
        "__RequestVerificationToken" = $token
        "ItemName" = "Temp Item for Delete"
        "Sku" = "TEMP-DEL-999"
        "StockQuantity" = "10"
        "Price" = "1.99"
    }
    try {
        [void](Invoke-WebRequest -Uri "$baseUrl/Inventory/Create" -Method Post -Body $createBody -WebSession $session -MaximumRedirection 0)
    } catch {
        # ignore redirect error
    }
    
    # Let's find the new item ID
    $listRes = Invoke-WebRequest -Uri "$baseUrl/Inventory" -WebSession $session
    $idPattern = '/Inventory/Delete/(\d+)">Delete</a>'
    if ($listRes.Content -match $idPattern) {
        # Find the last match or all matches
        $allMatches = [regex]::matches($listRes.Content, $idPattern)
        $lastId = $allMatches[$allMatches.Count - 1].Groups[1].Value
        Write-Output "Newly created inventory item ID to delete: $lastId"
        
        # GET delete page
        $getDelRes = Invoke-WebRequest -Uri "$baseUrl/Inventory/Delete/$lastId" -WebSession $session
        if ($getDelRes.Content -match $tokenPattern) {
            $token = $Matches[1]
            $delBody = @{
                "__RequestVerificationToken" = $token
                "Id" = $lastId
            }
            try {
                $postDelRes = Invoke-WebRequest -Uri "$baseUrl/Inventory/Delete/$lastId" -Method Post -Body $delBody -WebSession $session -MaximumRedirection 0
                Write-Output "POST Inventory/Delete status: $($postDelRes.StatusCode)"
            } catch {
                Write-Output "POST Inventory/Delete failed (expected 302 redirect): $_"
            }
        }
    }
}
