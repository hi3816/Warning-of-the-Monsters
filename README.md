# Warning-of-the-Monsters

## 📖 목차
1. [프로젝트 소개](#프로젝트-소개)
2. [팀소개](#팀소개)
3. [개발기간](#개발기간)
4. [주요기능](#주요기능)
5. [UML](#uml)
6. [기술스택](#기술스택)
7. [발표자료](#발표자료)
8. [라이선스](#라이선스)

---
## 프로젝트 소개
<img width="955" alt="Screenshot 2024-12-22 at 05 00 44" src="https://github.com/user-attachments/assets/519b7005-7b5b-4d3e-8ecc-821d65b8c7fa" />


### 스토리
```
몬스터와 인간이 존재하는 이 세계는 두려움과 오해로 가득 차 있습니다.
몬스터들은 친절하지만, 인간의 적대감 때문에 깊은 숲과 어두운 동굴과 같이 외진곳에 살고 있습니다.
사람들은 미지의 존재에 대한 두려움으로 몬스터를 위협으로 여기며 증오의 악순환이 이어집니다.

그런 가운데, 우리 몬스터 친구들은 호기심에 이끌려 사람이 사는 마을 근처로 내려갑니다.
하지만 그곳에서 마을주민과 마주치게 되고, 겁에 질린 사람들은 몬스터를 토벌하기 위해 숲으로 향합니다.
과연 우리 몬스터 친구들은 오해를 풀고 인간들과 깊은 유대를 형성할 수 있을까요?
```

### 장르 : 2D 타워 티펜스
```
목표: 인간이 몬스터 마을의 선을 넘지 않도록 겁을 줘 쫓아내세요.
몬스터: 몬스터는 타워처럼 배치되어 인간을 놀라게 하고 겁을 주는 역할을 합니다.
인간 공포수치력: 놀라면 인간의 공포가 증가합니다. 공포 바가 최대치에 도달하면 도망칩니다.
몬스터 피로도: 몬스터가 피로해지면 몬스터 마을로 돌아갑니다.
스킬 사용: 스킬 쿨타임이 0이 되면 특별한 스킬을 사용하여 몬스터들을 도와주세요.
```

## 팀소개

<a href="https://github.com/ESe0l">
  <img src="https://github.com/ESe0l&refresh=true">
</a> | <a href="https://github.com/00ovo00">
  <img src="https://github.com/00ovo00&refresh=true">
</a> | <a href="https://github.com/simple2126">
  <img src="https://github.com/simple2126&refresh=true">
</a> | <a href="https://github.com/hi3816">
  <img src="https://github.com/hi3816&refresh=true">
</a>

## 개발기간
2024.11.25 - 2025.01.20


<table style="width:85%">
  <tr>
    <th>기간</th>
    <th>완료한 작업</th>
  </tr>
  <tr>
    <td>2024.11.25 - 11.26</td>
    <td>기획</td>
  </tr>
  <tr>
    <td>2024.11.26 - 12.06</td>
    <td>스테이지, 인간, 몬스터 제작
    <br/>전투 시스템
    <br/>로비 및 게임 내 UI
    <br/>게임 결과(WIN, LOOSE)</td>
  </tr>
    <td>2024.12.09 - 12.13</td>
    <td>인간, 몬스터 추가
    <br/>몬스터 진화
    <br/>스킬 제작
    <br/>로비씬 -> 게임씬 선택된 몬스터 정보 보내기</td>
<tr>
  <td>2024.12.16 - 12.20</td>
  <td>디테일 추가
  <br/>기능 보완 및 오류 해결
  <br/>버그 잡기</td>
</tr>
  <tr>
  <td>2024.12.23 - 12.26</td>
  <td>프로젝트 보완 및 수정</td>
</tr>
  <tr>
  <td>2024.12.27 - 2025.01.03</td>
  <td>몬스터 타입 추가
  <br/>스테이지 추가
  <br/>몬스터 종류 추가
  <br/>인간 종류 추가
  <br/>게임 저장</td>
</tr>
  <tr>
  <td>2025.01.06 - 01.09</td>
  <td>밸런스 조절 및 배포 준비</td>
</tr>
  <tr>
  <td>2025.01.10 - 01.14</td>
  <td>유저 테스트</td>
</tr>
  <tr>
  <td>2025.01.15 - 01.20</td>
  <td>버그 및 개선점 보안</td>
</table>

## 주요기능
### ⚙️ FSM(Finite-State Machine)
  <table style="width:85%">
  <tr>
    <th>💡 사용 이유</th>
    <th> </th>
    </tr>
    <tr>
      <td>단순하고 명확성이 높다</td>
      <td>FSM을 사용하면 구조를 이해하고 구현하기 쉬우며, 상태와 상태 간 전환이 명확해 디버깅과 유지보수에 용이</td>
  </tr>
      <td>모듈화</td>
      <td>각 상태가 독립적으로 관리되어 수정 및 확장이 쉽움</td>
  </tr>
      <td>예측 가능한 동작</td>
      <td>한번에 하나의 상태만 활성화되어 비정상적인 행동을 방지</td>
  </table>

### ⚙️ UI동적 생성
  <table style="width:85%">
  <tr>
    <th>💡 사용 이유</th>
    <th> </th>
    </tr>
    <tr>
      <td>효율적인 UI 관리</td>
      <td>게임 내 다양한 UI 요소(팝업, HUD 등)가 필요하지만, 모든 UI를 미리 배치하면 관리가 복잡해짐.
      <br/>Show<T>(), ShowPopup<T>() 메서드를 통해 필요한 UI를 동적으로 생성.</td>
  </tr>
      <td>메모리 사용 최적화</td>
      <td>모든 UI를 미리 생성해두면 불필요한 메모리 사용이 증가.
      <br/>PoolManager와 연결해 UI 객체 풀링으로 메모리 낭비를 줄임.
      <br/>UI 생성 시 Resources.Load를 사용해 필요한 프리팹만 로드하고, 사용 후 Destroy()로 제거.</td>
  </tr>
      <td>유연성과 확장성</td>
      <td>다양한 해상도와 화면 비율에 대응해야 하거나, UI 요소가 자주 변경되는 경우 필요.
      <br/>CanvasScaler를 활용해 동적으로 생성된 UI가 화면 크기에 맞게 스케일 조정.
      <br/>UIBase를 기반으로 다양한 UI 프리팹을 동적으로 확장 가능.</td>
  </table>

### ⚙️ UGS로 데이터 관리
  <table style="width:85%">
  <tr>
    <th>💡 사용 이유</th>
    </tr>
    <tr>
      <td>구글 스프레드 시트로 데이터를 관리하기에 접근성이 좋고 직관적임</td>
  </tr>
      <td>접근 제한도 쉽게 제어할 수 있음.</td>
  </tr>
      <td>게임 데이터를 실시간으로 수정하여 테스트할 수 있음</td>
  </table>

### ⚙️ 오브젝트 풀
  <table style="width:85%">
    <tr>
      <th>💡 사용 이유</th>
      <th> </th>
      </tr>
      <tr>
        <td>성능 최적화</td>
        <td>객체를 자주 생성하고 파괴하는 것은 성능에 큰 영향을 미침.
        <br/>C#의 Garbage Collector(GC)는 객체를 파괴할 때 추가적인 연산을 발생시켜 게임의 프레임 드랍을 유발할 수 있음.
        <br/>객체를 미리 생성하여 런타임에서 생성과 파괴의 오버헤드 제거.</td>
    </tr>
        <td>초기화 시간 감소</td>
        <td>객체를 미리 초기화해두면 런타임에서 다시 초기화하는 시간을 절약할 수 있음.
        <br/>객체가 이미 초기화된 상태에서 풀에서 꺼내기만 하면 되므로 초기화 시간이 줄어듦.</td>
    </tr>
        <td>코드 가독성과 유지보수성 향상 </td>
        <td>객체 관리가 체계적으로 이루어지기 때문에 코드가 더 깔끔하고 유지보수하기 쉬움.
        <br/>객체 생성/파괴를 여러 곳에서 관리 → 코드가 복잡해지고 버그 발생 가능성 증가.
        <br/>오브젝트 풀 사용 → 객체 관리가 단일화되어 코드가 간결해짐.</td>
    </table>

## UML

### 🧍🏻‍♂️ Human
![HumanUML](https://github.com/user-attachments/assets/526721d7-5488-4eea-a0b2-d27caf4d0372)

### 👾 Monster
![MonsterUML](https://github.com/user-attachments/assets/bbb2ba65-2b0d-44f2-9377-6c0b2cfa4dbf)

### 🎨 UI
![UIUML](https://github.com/user-attachments/assets/1649e0f4-0c0b-471f-9aa9-52ce189b5d5a)

### 🖥️ Manager
![ManagerUML](https://github.com/user-attachments/assets/6ccffda0-2838-4528-b40e-9db7806cf87b)

---
## 기술스택

- Language: C#


- Version Control: Unity 2022.3.17f1


- IDE: Visual Studio, Rider


- Framework: .NET SDK version 8.0.401

## 발표자료
[팀 노션](https://gorgeous-mousepad-03c.notion.site/7-14942b67318780e79804d46f90aecb77?pvs=4)


[WireFrame](https://www.figma.com/board/7LOSL81sfmvUHM4C1jvtXd/Warning-of-the-Monsters?node-id=0-1&t=t2NWAwBbt9yvN3Az-1)


[영상](https://youtu.be/Jy18rLKbkuQ?si=BzcynBvvltqYMn6c)


[PPT](https://www.canva.com/design/DAGb5SnAQJA/e7GRr4RFCZTXE9FteMqezw/view?utm_content=DAGb5SnAQJA&utm_campaign=designshare&utm_medium=link2&utm_source=uniquelinks&utlId=hf95b7dd875)

## 라이선스
[타일1](https://www.gamedevmarket.net/asset/fantasy-rpg-tileset-pack-3541), [타일2](https://www.gamedevmarket.net/asset/50-pixel-art-textures) 


[사람](https://www.gamedevmarket.net/asset/over-80-rpg-characters-w-animations-3540)


[몬스터1](https://www.gamedevmarket.net/asset/faction-expansion-woodland-folk), [몬스터2](https://www.gamedevmarket.net/asset/monsters-time-fantasy-rpg-sprite-pack-4391), [몬스터3](https://www.gamedevmarket.net/asset/fantasy-rpg-monster-pack) 


[UI](https://www.gamedevmarket.net/asset/rpg-ui-pack)


[이펙트1](https://www.gamedevmarket.net/asset/super-pixel-explosion-fx-pack-1-pixel-art-effect-animations), [이펙트2](https://www.gamedevmarket.net/asset/super-pixel-effects-pack-3-pixel-art-effect-animations)


[글씨체](https://noonnu.cc/font_page/800)


[사운드](https://www.gamedevmarket.net/asset/2000-game-sound-effects)
