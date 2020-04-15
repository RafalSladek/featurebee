loadLibrary "as24-fizz-microsoft-library@master" // Load library
def deploy_agent_prod = "deploy-as24-cashstack-shared-services-prod"
def deploy_agent_dev = "deploy-as24-cashstack-shared-services-dev"
def master_branch = "master"

pipeline {
  agent none
  options {
    disableConcurrentBuilds()
    buildDiscarder(logRotator(numToKeepStr: '10', artifactNumToKeepStr: '10'))
  }

  environment {
    SLACK_CHANNEL = 'tiger-muc-events' // send build notifications
    MASTER_BRANCH = "${master_branch}"
    BUILD_NUMBER = getBuildVersion('dateassembly') // use ms assembly compatible version
    BUILD_ID = getBuildVersion('dateassembly', true) // use with unique build identifier for nuget package
    AWS_DEFAULT_REGION = "eu-central-1"
  }

  post {
    aborted {
      aborted()
    }
    failure {
      failed()
    }
    success {
      finished()
    }
  }

  stages {
    stage('build') {
      agent { label 'windows-build' }
      when {
        beforeAgent true
        branch master_branch
      }
      steps {
        script {
          bat 'build.cmd'
          withNuget('as24', [Verbosity: 'Detailed']) { nuget ->
              // push to registry source
              nuget.push("packaging/FeatureBee.${env.BUILD_ID}.nupkg")
          }
        }
      }
    }
  }
}

def deploy(stage) {
  withEnv(["STAGING=${stage}"]) {
    ansiColor('xterm') {
      echo 'deploy stack'
      sh 'deploy/deploy.sh'
    }
  }
}

def sendMessage(color, prefix, msg) {
  script {
    if (env.BRANCH_NAME == "${env.MASTER_BRANCH}") {
      slackSend channel: "${env.SLACK_CHANNEL}",
        color: color,
        message: "${prefix}Deployment `${currentBuild.fullDisplayName}` ${msg}.\n(<${env.BUILD_URL}|Open>)"
    }
  }
}

def approval() {
  sendMessage('#439FE0', '', 'is waiting approval')
  input 'Continue?'
}

def aborted() {
  sendMessage('warning', '', 'aborted')
}

def failed() {
  sendMessage('danger', '@here :bomb: ', 'failed')
}

def finished() {
  sendMessage('good', ':+1: ', 'finished')
}