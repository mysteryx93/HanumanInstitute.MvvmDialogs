#!/bin/bash

find . -type d -name bin -prune -exec rm -rf {} \;
find . -type d -name obj -prune -exec rm -rf {} \;
