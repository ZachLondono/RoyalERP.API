import sys
import requests
from tabulate import tabulate

class Tester:

        base_url = 'http://localhost:7777/'
        headers = ['endpoint', 'expected', 'result']
        data = []

        test_count = 0
        failed_count = 0

        def checkresult(self, url,  r, expected_status):
                passed = False
                if r.status_code == expected_status:
                        passed = True
                        status_msg = '\033[92m'
                else:
                        passed = False
                        status_msg = '\033[91m'

                status_msg = status_msg + str(r.status_code)

                status_msg = status_msg + '\033[0m'
                self.data.append([url, expected_status, status_msg])

                if not passed:
                        self.failed_count += 1
                self.test_count += 1

                return passed

        def getrequest(self, url, expected_status):
                try:
                        r = requests.get(url = self.base_url+url, headers={'X-API-KEY':'A1C33C48-CF3A-4DAB-8EB3-CB76976B5690'})
                        self.checkresult(url, r, expected_status)
                except BaseException as er:
                        status_msg = '\033[91m' + 'Connection Error'
                        print(er)

        def postrequest(self, url, body, expected_status):
                try:
                        r = requests.post(url = self.base_url+url, headers={'X-API-KEY':'A1C33C48-CF3A-4DAB-8EB3-CB76976B5690'}, json=body)
                        self.checkresult(url, r, expected_status)
                except BaseException as er:
                        status_msg = '\033[91m' + 'Connection Error'
                        print(er)

tester = Tester()

tester.getrequest('orders/', 200)
tester.getrequest('workorders/', 200)
tester.getrequest('companies/', 200)
tester.getrequest('products/', 200)
tester.getrequest('attributes/', 200)
tester.getrequest('productclasses/', 200)
tester.getrequest('does_not_exist/', 404)

new_order = {
        "Number" : "OT000",
        "Name" : "Abc123",
        "CustomerName" : "Company A",
        "VendorName" : "Company B",
}
tester.postrequest('orders/', new_order, 201)

new_workorder = {
        "SalesOrderId" : "d49bf0cb-3aa9-419d-8cbb-01539a9c68a7",
        "Number" : "Ot000",
        "Name" : "Order Name",
        "CustomerName" : "Customer Abc",
        "VendorName" : "Company A",
        "ProductClass" : "d49bf0cb-3aa9-419d-8cbb-01539a9c68a7",
        "Quantity" : "5"
}
tester.postrequest('workorders/', new_workorder, 201)

new_company = {
        "Name" : "Abc123"
}
tester.postrequest('companies/', new_company, 201)

new_product = {
    "Name" : "New Product"
}
tester.postrequest('products/', new_product, 201)

new_attribute = {
    "Name" : "New Attribute"
}
tester.postrequest('attributes/', new_attribute, 201)

new_productclass = {
    "Name" : "New Product Class"
}
tester.postrequest('productclasses/', new_productclass, 201)

print(tabulate(tester.data, headers=tester.headers))

print("----------------------------------")
print('Test Count\t' + str(tester.test_count))
print('\033[92m' + 'Passed\t\t' + str(tester.test_count - tester.failed_count) + '/' + str(tester.test_count) + '\033[0m')
print('\033[91m' + 'Failed\t\t' + str(tester.failed_count) + '/' + str(tester.test_count) + '\033[0m')
print("----------------------------------")

result = 0 if tester.failed_count == 0 else 1

exit(result)