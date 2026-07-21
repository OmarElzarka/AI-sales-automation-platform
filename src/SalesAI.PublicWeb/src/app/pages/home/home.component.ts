import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UiService } from '../../services/ui.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  constructor(private uiService: UiService) {}

  openDemoModal() {
    this.uiService.openDemoModal();
  }

  features = [
    {
      icon: '⚡',
      title: 'Workflow Automation',
      description: 'Automate repetitive tasks across your entire organization with intelligent triggers and actions.'
    },
    {
      icon: '📊',
      title: 'Smart Analytics',
      description: 'Real-time dashboards and predictive insights powered by machine learning algorithms.'
    },
    {
      icon: '🤖',
      title: 'AI Copilot',
      description: 'An AI assistant that learns your patterns, suggests optimizations, and executes autonomously.'
    },
    {
      icon: '👥',
      title: 'Team Collaboration',
      description: 'Seamless cross-team workflows with shared automations, comments, and approval chains.'
    },
    {
      icon: '🔒',
      title: 'Enterprise Security',
      description: 'SOC 2 compliant with end-to-end encryption, SSO, RBAC, and comprehensive audit trails.'
    },
    {
      icon: '🔌',
      title: '200+ Integrations',
      description: 'Connect with Salesforce, HubSpot, Slack, Jira, and hundreds more — out of the box.'
    }
  ];

  steps = [
    { number: '01', title: 'Connect', description: 'Integrate your existing tools and data sources in minutes, not weeks.' },
    { number: '02', title: 'Automate', description: 'Build intelligent workflows with our visual designer or let AI generate them.' },
    { number: '03', title: 'Optimize', description: 'AI continuously analyzes and improves your workflows for peak performance.' },
    { number: '04', title: 'Scale', description: 'Grow from 10 to 10,000 users without changing a thing. We scale with you.' }
  ];

  pricingPlans = [
    {
      name: 'Starter',
      price: '$49',
      period: '/month',
      description: 'Perfect for small teams getting started with automation.',
      features: ['Up to 10 users', '1,000 automations/mo', '50+ integrations', 'Email support', 'Basic analytics', 'Community access'],
      cta: 'Start Free Trial',
      popular: false
    },
    {
      name: 'Professional',
      price: '$149',
      period: '/month',
      description: 'For growing teams that need advanced AI capabilities.',
      features: ['Up to 50 users', 'Unlimited automations', '200+ integrations', 'AI Copilot included', 'Advanced analytics', 'Priority support', 'Custom workflows', 'API access'],
      cta: 'Start Free Trial',
      popular: true
    },
    {
      name: 'Enterprise',
      price: 'Custom',
      period: '',
      description: 'For large organizations with complex requirements.',
      features: ['Unlimited users', 'Unlimited everything', 'Custom integrations', 'Dedicated AI models', 'White-glove onboarding', '24/7 premium support', 'SLA guarantee', 'On-premise option'],
      cta: 'Contact Sales',
      popular: false
    }
  ];

  testimonials = [
    {
      quote: 'NovaFlow cut our manual data entry by 85%. Our sales team now focuses on what they do best — selling.',
      author: 'Sarah Mitchell',
      role: 'VP of Sales, TechCorp',
      avatar: 'SM'
    },
    {
      quote: 'The AI Copilot is like having an extra team member. It catches things we miss and suggests improvements we never thought of.',
      author: 'James Rodriguez',
      role: 'CTO, ScaleUp Labs',
      avatar: 'JR'
    },
    {
      quote: 'We evaluated 12 platforms before choosing NovaFlow. Nothing else comes close in terms of AI capabilities and ease of use.',
      author: 'Emily Chen',
      role: 'Head of Ops, DataFirst',
      avatar: 'EC'
    }
  ];

  faqs = [
    {
      question: 'How long does it take to set up NovaFlow?',
      answer: 'Most teams are up and running within a day. Our onboarding wizard and pre-built templates make it easy to get started. Enterprise deployments typically take 1-2 weeks with our dedicated team.',
      open: false
    },
    {
      question: 'Does NovaFlow integrate with my existing tools?',
      answer: 'Yes! We support 200+ integrations out of the box, including Salesforce, HubSpot, Slack, Microsoft 365, Google Workspace, Jira, and more. Our API also allows custom integrations.',
      open: false
    },
    {
      question: 'How does the AI Copilot work?',
      answer: 'Our AI Copilot learns from your team\'s workflow patterns, identifies bottlenecks, and suggests optimizations. It can also auto-generate workflows, draft communications, and predict outcomes based on historical data.',
      open: false
    },
    {
      question: 'Is my data secure?',
      answer: 'Absolutely. NovaFlow is SOC 2 Type II certified with end-to-end encryption, role-based access control, and comprehensive audit trails. We also support SSO and on-premise deployment for enterprise customers.',
      open: false
    },
    {
      question: 'Can I try NovaFlow before committing?',
      answer: 'Yes! We offer a 14-day free trial on all plans with no credit card required. You can also request a personalized demo to see how NovaFlow fits your specific use case.',
      open: false
    },
    {
      question: 'What kind of support do you offer?',
      answer: 'Starter plans include email support. Professional plans get priority support with <4hr response times. Enterprise customers receive 24/7 premium support with a dedicated success manager.',
      open: false
    }
  ];

  stats = [
    { value: '10,000+', label: 'Teams worldwide' },
    { value: '50M+', label: 'Automations run' },
    { value: '99.99%', label: 'Uptime SLA' },
    { value: '85%', label: 'Time saved' }
  ];

  toggleFaq(index: number) {
    this.faqs[index].open = !this.faqs[index].open;
  }
}
