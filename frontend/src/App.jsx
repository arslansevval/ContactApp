import AppRouter from "./router/AppRouter";
import './App.css';
import ErrorBoundary from "./components/ErrorBoundary"; // doğru path ile import et

function App() {
  return (
    <ErrorBoundary>
      <AppRouter />
    </ErrorBoundary>
  );
}

export default App;
