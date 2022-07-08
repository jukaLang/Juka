$certname = "Juka" 
$cert = New-SelfSignedCertificate -Subject "CN=$certname" -CertStoreLocation "./" -KeyExportPolicy Exportable -KeySpec Signature -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256
Export-Certificate -Cert $cert -FilePath "src\$certname.cer"   ## Specify your preferred location
$mypwd = ConvertTo-SecureString -String "jukatest" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "src\$certname.pfx" -Password $mypwd