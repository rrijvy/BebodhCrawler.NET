import requests


class CustomRequest:
    def make_get_request(url):
        response = requests.get(url)
        return response


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
