#!/usr/bin/env python

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import httplib
import os
import sys
import ssl

class FleetManagerClient:

	def __init__(self, fleetmanager_url, version):
		print("Fleet Manager Host is: " + fleetmanager_url)
		self.fleetmanager_url = fleetmanager_url
		self.fleetmanager_version = version

	def get_fleetmanager_host(self):
		return self.fleetmanager_url

	def get_robot_config(self, robotId):
		print("Robot id is: " + robotId)

		host = self.get_fleetmanager_host()
		url = "/api/v" + str(self.fleetmanager_version) + "/Robots/" + str(robotId) + "/connection"

		conn = httplib.HTTPSConnection(host, context=ssl._create_unverified_context())
		headers={"Accept": "application/json", "Content-Length": "0"}
		conn.request("PUT", url, None, headers)
		response = conn.getresponse()
		iot_connection_string = response.read()
		print(response.status, response.reason, iot_connection_string)
		conn.close()

		return iot_connection_string
