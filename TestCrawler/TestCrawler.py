import json
import sys
import pymongo
from selenium import webdriver
import time
import os
import requests
from PythonLibs import CustomRequest
from BrowserUserAgent import BrowserUserAgent


arguments = sys.argv[1:]

filename = f"{int(time.time())}.csv"

export_file = os.path.join(
    os.path.dirname(os.path.realpath(__file__)), "exports", filename
)


# url = "http://localhost:28851/api/Crawls/UpdateCrawls"

# payload = json.dumps(
#     {
#         "crawlerName": "AmazonCrawlerService 1",
#         "outputPath": str(export_file),
#         "progress": [
#             {"at": str(time.time()), "progress": "20"},
#             {"at": str(time.time()), "progress": "40"},
#         ],
#     }
# )
# headers = {"Content-Type": "application/json"}
# response = requests.request("POST", url, headers=headers, data=payload)
# print(response.text)


parsed_argument = {
    "category_name": "Camera & Photo",
    "category_url": "https://www.amazon.com/s?bbn=16225009011&rh=i%3Aspecialty-aps%2Cn%3A%2116225009011%2C",
    "sub_categories": [],
}


response = CustomRequest.make_get_request(
    parsed_argument["category_url"],
    headers={"User-Agent": BrowserUserAgent.GetRandom()},
)

print(response)
