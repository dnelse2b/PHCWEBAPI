using SGOFWS.DTOs;

namespace SGOFWS.Domains.Contracts;

public class WebTransactionCodes
{
	public static ResponseCodesDTO SUCCESS = new ResponseCodesDTO("0000", "Success");

	public static ResponseCodesDTO INCORRECTHTTP = new ResponseCodesDTO("0001", "Incorrect HTTP method");

	public static ResponseCodesDTO INVALIDJSON = new ResponseCodesDTO("0002", "Invalid JSON");

	public static ResponseCodesDTO INCORRECTAPIKEY = new ResponseCodesDTO("0003", "Incorrect API Key");

	public static ResponseCodesDTO APIKEYNOTFOUND = new ResponseCodesDTO("0004", "Api Key not provided");

	public static ResponseCodesDTO INVALIDREFERENCE = new ResponseCodesDTO("0005", "Invalid Reference");

	public static ResponseCodesDTO DUPLICATEDPAYMENT = new ResponseCodesDTO("0006", "Duplicated payment");

	public static ResponseCodesDTO INTERNALERROR = new ResponseCodesDTO("0007", "Internal Error");

	public static ResponseCodesDTO INVALIDAMOUNT = new ResponseCodesDTO("0008", "Invalid Amount Used");

	public static ResponseCodesDTO MUSTPROVIDEREQUESTID = new ResponseCodesDTO("0009", "Request Id not provided");

	public static ResponseCodesDTO USERALREADYEXISTS = new ResponseCodesDTO("0010", "User already exists!");

	public static ResponseCodesDTO USERCREATIONFAILED = new ResponseCodesDTO("0011", "User creation failed! Please check user details and try again.");
}
