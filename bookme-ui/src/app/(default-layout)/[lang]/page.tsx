const Home = ({ params }: { params: { lang: string } }) => {
  return <div>{params.lang}</div>;
};

export default Home;
