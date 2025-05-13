Info to know for future development

Deployment steps: 
	1. Click publish to folder
	2. Scan folder for Viruses
	2. Zip folder minus appsettings and web.config files
	3. drag and drop onto deploymnett DVD
	4. Burn files to DVD
	5. Put DVD in CSNI then copy zip to desktop
	6. Remote into app server, then move zip there
	7. unzip
	8. Make a copy of D:/VirtualWebs/SMART as a backup
	9. paste and overwrite new code folder contents into D:/VirtualWebs/SMART
		-if windows says the files are in use, stop the website on IIS, try again, then restart the website

When publishing to CSNI, do not include appsettings and web.config files. Leave the ones currently there.
appsettings contains the connection string, and overwrite windows authentication settings for IIS.
If websit cannot read user info, make sure the website config in IIS is set with windows authentication enabled and Anonymous Authentication disabled.

To regenerate dbcontext and models put this command in nugget package manager console.
Scaffold-DbContext "Data Source=JFC-LTM-VMW2K16\JIIFC;Initial Catalog=SMARTV3;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -force