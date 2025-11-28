# Jabil - UI JavaScript

[![Conventional Commits][badge/media/conventional-commits]][badge/conventional-commits]
[![Build Status][badge/media/build-status]][badge/build-status]

## `@jabil/ui-js`

1. [Contribution Guide](#contribution-guide)

### Contribution Guide

#### Pre-requisites

- NodeJS 12.x installed locally (https://nodejs.org)
- Knowledge/understanding of the `npm link` command (https://docs.npmjs.com/cli/link)

#### Run Steps

1. Clone this repository
1. `cd` into the repository
1. Run `npm install`
1. Run `npm run build`
1. Run `npm link --only=production`
1. `cd` into a consumer project directory, then run `npm link @jabil/ui-js`

#### Further Resources

- Please read the contribution guide on the [UI/UX Community Wiki][wiki]

[wiki]: https://dev.azure.com/jblinnersource/UIUX%20Community/_wiki/wikis/UIUX%20Community
[badge/media/conventional-commits]: https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg
[badge/conventional-commits]: https://conventionalcommits.org
[badge/media/build-status]: https://dev.azure.com/jblinnersource/UIUX%20Community/_apis/build/status/ui-js?branchName=main
[badge/build-status]: https://dev.azure.com/jblinnersource/UIUX%20Community/_build/latest?definitionId=14&branchName=main
