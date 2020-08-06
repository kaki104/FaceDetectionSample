# Post
https://kaki104.tistory.com/662

# Youtube
- Part 1
  https://youtu.be/wxIFGmG6JCo
- Part 2
  

# Face Detection API
. Real-time face recognition with Microsoft Cognitive Services
. Google
  - Vision API
    + Facial Detection 
      - Free : First 1000 units/month 
      - $1.5 for 1000 unit : 1001 – 5,000,000 / month
. Microsoft Azure
  - Face API
    + Face Detection ~ Face Identification
      - Free : 20 transactions for minute
      - $1.0 for 1000 transactions : 0 – 1M transactions
. OpenCV
. Dlib C++ Library

# OpenCV
. 오픈 소스 컴퓨터 비전 라이브러리 중 하나로 크로스플랫폼과 실시간 이미지 프로세싱에 중점을 둔 라이브러리.
. Windows, Linux, OS X(macOS), iOS, Android 등 다양한 플랫폼을 지원
. OpenCvSharp
  - C# : OpenCvSharp4 nuget package
  - UWP : OpenCvSharp4.runtime.uwp nuget package
  - haarcascade_frontalface_alt.xml

# OpenCVSharp
. Mat class 
  - Matrix의 약자로 행렬을 표현하기 위한 데이터 형식
. Haar Cascade
  - 이미지 또는 비디오에서 객체를 식별하는데 사용되는 기계 학습 객체 감지 알고리즘
  - haarcascade_frontalface_alt.xml : 얼굴 전면 식별
  - DetectMultiScale
    + 입력된 이미지에서 크기가 다른 물체를 감지합니다. 감지 된 객체는 사각형 목록으로 반환
