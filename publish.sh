#!/usr/bin/env bash

if [[ "$CIRRUS_RELEASE" == "" ]]; then
  echo "Not a release. No need to deploy!"
  exit 0
fi

if [[ "$GITHUB_TOKEN" == "" ]]; then
  echo "Please provide GitHub access token via GITHUB_TOKEN environment variable!"
  exit 1
fi

file_content_type="application/octet-stream"
if [[ "$JUKA_TOKEN" == "Unix_Amd64" ]]; then
files_to_upload=(
  "Juka_Unix_Amd64_${CIRRUS_TAG}.tar.gz"
  "JukaAPI_Unix_Amd64_${CIRRUS_TAG}.tar.gz"
)
fi

if [[ "$JUKA_TOKEN" == "Linux_X86" ]]; then
files_to_upload=(
  "Juka_Linux_X86_${CIRRUS_TAG}.tar.gz"
  "JukaAPI_Linux_X86_${CIRRUS_TAG}.tar.gz"
)
fi

for fpath in "${files_to_upload[@]}"
do
  echo "Uploading $fpath..."
  name=$(basename "$fpath")
  url_to_upload="https://uploads.github.com/repos/$CIRRUS_REPO_FULL_NAME/releases/$CIRRUS_RELEASE/assets?name=$name"
  curl -X POST \
    --data-binary @$fpath \
    --header "Authorization: token $GITHUB_TOKEN" \
    --header "Content-Type: $file_content_type" \
    $url_to_upload
done